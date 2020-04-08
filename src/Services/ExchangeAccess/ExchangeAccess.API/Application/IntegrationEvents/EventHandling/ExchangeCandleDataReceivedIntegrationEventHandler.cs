using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeCandleDataReceivedIntegrationEventHandler : IIntegrationEventHandler<ExchangeCandleDataReceivedIntegrationEvent>
    {
        private readonly ICandleChartRepository _candleChartRepository;
        private readonly IExchangeAccessIntegrationEventService _exchangeAccessIntegrationEventService;

        public ExchangeCandleDataReceivedIntegrationEventHandler(ICandleChartRepository candleChartRepository, IExchangeAccessIntegrationEventService exchangeAccessIntegrationEventService)
        {
            _candleChartRepository = candleChartRepository ?? throw new ArgumentNullException(nameof(candleChartRepository));
            _exchangeAccessIntegrationEventService = exchangeAccessIntegrationEventService ?? throw new ArgumentNullException(nameof(exchangeAccessIntegrationEventService));
        }

        public async Task Handle(ExchangeCandleDataReceivedIntegrationEvent @event)
        {
            //return;
            try
            {
                var baseCurrency = @event.BaseCurrency.ToUpper();
                var quoteCurrency = @event.QuoteCurrency.ToUpper();
                var exchangeId = @event.ExchangeId;
                var candleChartId = @event.CandleChartId;

                var existingCandleChart = await _candleChartRepository.GetByCandleChartIdAsync(candleChartId);
                if (existingCandleChart == null)
                {
                    throw new KeyNotFoundException($"Candle chart with Id \"{candleChartId}\" not found.");
                }

                var candle = @event.Candle;


                existingCandleChart.UpdateCandle(
                    candle.Timestamp,
                    candle.High,
                    candle.Low,
                    candle.Open,
                    candle.Close,
                    candle.Volume
                    );

                _candleChartRepository.Update(existingCandleChart);

                try
                {
                    await _candleChartRepository.UnitOfWork.SaveEntitiesAsync();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("Exception ignored: DbUpdateConcurrencyException.");
                    Console.WriteLine("Error message: " + ex.Message);
                }

                await this._exchangeAccessIntegrationEventService.PublishThroughEventBusAsync(new CandleChartUpdatedIntegrationEvent(
                    @event.CandleChartId,
                    existingCandleChart.ExchangeId,
                    existingCandleChart.BaseCurrency,
                    existingCandleChart.QuoteCurrency,
                    existingCandleChart.CandlePeriod.Name,
                    candle.Timestamp,
                    candle.High,
                    candle.Low,
                    candle.Open,
                    candle.Close,
                    candle.Volume
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: ExchangeCandleDataReceivedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }





            /*var saved = false;
            while (!saved)
            {
                try
                {
                    await _candleChartRepository.UnitOfWork.SaveEntitiesAsync();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    foreach (var entry in ex.Entries)
                    {

                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues != null ? databaseValues[property] : proposedValue;

                            // TODO: decide which value should be written to database
                            proposedValues[property] = proposedValue;
                        }
                    }

                    Console.WriteLine("Exception Solved: DbUpdateConcurrencyException.");
                }
            }*/

        }
    }
}

