using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.EventHandling
{
    public class TargetPriceCandleDataRequestedIntegrationEventHandler : IIntegrationEventHandler<TargetPriceCandleDataRequestedIntegrationEvent>
    {
        private readonly ICandleChartRepository _candleChartRepository;
        private readonly IExchangeAccessIntegrationEventService _exchangeAccessIntegrationEventService;
        private readonly IExchangeAccessService _exchangeAccessService;

        public TargetPriceCandleDataRequestedIntegrationEventHandler(ICandleChartRepository candleChartRepository, IExchangeAccessIntegrationEventService exchangeAccessIntegrationEventService, IExchangeAccessService exchangeAccessService)
        {
            _candleChartRepository = candleChartRepository ?? throw new ArgumentNullException(nameof(candleChartRepository));
            _exchangeAccessIntegrationEventService = exchangeAccessIntegrationEventService ?? throw new ArgumentNullException(nameof(exchangeAccessIntegrationEventService));
            _exchangeAccessService = exchangeAccessService ?? throw new ArgumentNullException(nameof(exchangeAccessService));
        }

        public async Task Handle(TargetPriceCandleDataRequestedIntegrationEvent @event)
        {
            try
            {
                var exchangeId = @event.ExchangeId;
                var baseCurrency = @event.BaseCurrency;
                var quoteCurrency = @event.QuoteCurrency;
                var period = CandlePeriod.FromName(@event.CandlePeriod);

                //DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                var from = @event.From;
                var to = @event.To;

                var result = new List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle>();


                var candleChart = await _candleChartRepository.GetByCurrencyPairAsync(baseCurrency, quoteCurrency, exchangeId, period);

                if (candleChart != null)
                {
                    var updatedChart = candleChart;
                    DateTime fromWithWarmingPeriod = from;


                    if (!candleChart.HasCompleteCandlesBetween(fromWithWarmingPeriod, to))
                    {
                        var candles = await this._exchangeAccessService.GetCandlesData(
                            exchangeId,
                            baseCurrency,
                            quoteCurrency,
                            period,
                            fromWithWarmingPeriod,
                            to);

                        foreach (var candle in candles)
                        {
                            candleChart.UpdateCandle(candle.Timestamp, candle.High, candle.Low, candle.Open, candle.Close, candle.Volume);
                        }

                        updatedChart = this._candleChartRepository.Update(candleChart);
                        await this._candleChartRepository.UnitOfWork.SaveEntitiesAsync();
                    }

                    var candlesData = updatedChart.GetCandles(fromWithWarmingPeriod, to);




                    foreach (var candle in candlesData)
                    {
                        result.Add(new CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle
                        {
                            Timestamp = candle.Timestamp,
                            Open = candle.Open,
                            Close = candle.Close,
                            High = candle.High,
                            Low = candle.Low,
                            Volume = candle.Volume
                        });
                    }
                }

                await this._exchangeAccessIntegrationEventService.PublishThroughEventBusAsync(
                   new TargetPriceCandleDataCreatedIntegrationEvent(
                       @event.TraceId,
                       @event.RoundtripId,
                       @event.CandlePeriod,
                       @event.ExchangeId,
                       @event.BaseCurrency,
                       @event.QuoteCurrency,
                       @event.HitPrice,
                       result));
            }
            catch(Exception ex)
            {

                Console.WriteLine("Handle Integration Event: TargetPriceCandleDataRequestedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
         
        }
    }
}
