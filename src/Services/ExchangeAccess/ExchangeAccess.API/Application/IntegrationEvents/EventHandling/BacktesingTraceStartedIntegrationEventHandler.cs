using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.API.Extensions;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.EventHandling
{
    public class BacktesingTraceStartedIntegrationEventHandler : IIntegrationEventHandler<BacktestingTraceStartedIntegrationEvent>
    {
        private readonly ICandleChartRepository _candleChartRepository;
        private readonly IExchangeAccessIntegrationEventService _exchangeAccessIntegrationEventService;
        private readonly IExchangeAccessService _exchangeAccessService;

        public BacktesingTraceStartedIntegrationEventHandler(
            ICandleChartRepository candleChartRepository,
            IExchangeAccessIntegrationEventService exchangeAccessIntegrationEventService,
            IExchangeAccessService exchangeAccessService
            )
        {
            this._candleChartRepository = candleChartRepository ?? throw new ArgumentNullException(nameof(candleChartRepository));
            this._exchangeAccessIntegrationEventService = exchangeAccessIntegrationEventService ?? throw new ArgumentNullException(nameof(exchangeAccessIntegrationEventService));
            this._exchangeAccessService = exchangeAccessService ?? throw new ArgumentNullException(nameof(exchangeAccessService));
        }

        public async Task Handle(BacktestingTraceStartedIntegrationEvent @event)
        {
            try
            {
                var exchangeId = @event.ExchangeId;
                var baseCurrency = @event.BaseCurrency;
                var quoteCurrency = @event.QuoteCurrency;


                //DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                var from = @event.From;
                var to = @event.To;

                var result = new Dictionary<string, List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle>>();

                foreach (var period in CandlePeriod.List())
                {
                    var candleChart = await _candleChartRepository.GetByCurrencyPairAsync(baseCurrency, quoteCurrency, exchangeId, period);

                    if (candleChart != null)
                    {
                        var updatedChart = candleChart;
                        DateTime fromWithWarmingPeriod = from;
                        var oneCandleMinutes = CandlePeriodService.GetOneCandleMinutesByPeriod(period);
                        fromWithWarmingPeriod = fromWithWarmingPeriod.AddMinutes(-oneCandleMinutes);

                        if (@event.MinAmountOfCandles.TryGetValue(period.Name, out int warmingPeriod))
                        {

                            fromWithWarmingPeriod = fromWithWarmingPeriod.AddMinutes(-(oneCandleMinutes * warmingPeriod));
                        }


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


                        var data = new List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle>();

                        foreach (var candle in candlesData)
                        {
                            data.Add(new CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle
                            {
                                Timestamp = candle.Timestamp,
                                Open = candle.Open,
                                Close = candle.Close,
                                High = candle.High,
                                Low = candle.Low,
                                Volume = candle.Volume
                            });
                        }

                        result.Add(period.Name, data);
                    }
                }

                await this._exchangeAccessIntegrationEventService.PublishThroughEventBusAsync(
                        new BacktestingDataCreatedIntegrationEvent(
                            @event.TraceId,
                            @event.InvestmentId,
                            @event.ExchangeId,
                            @event.BaseCurrency,
                            @event.QuoteCurrency,
                            @event.From.ToTimestamp(),
                            @event.To.ToTimestamp(),
                            result));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Handle Integration Event: BacktestingTraceStartedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
      
        }


    }
}
