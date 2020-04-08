using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.EventHandling
{
    public class TargetPriceCandleDataCreatedIntegrationEventHandler : IIntegrationEventHandler<TargetPriceCandleDataCreatedIntegrationEvent>
    {
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public TargetPriceCandleDataCreatedIntegrationEventHandler(ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(TargetPriceCandleDataCreatedIntegrationEvent @event)
        {
            try
            {
                var candles = new List<Candle>();

                foreach (var candle in @event.Candles)
                {
                    candles.Add(new Candle(
                                      candle.Timestamp,
                                      candle.High,
                                      candle.Low,
                                      candle.Open,
                                      candle.Close,
                                      candle.Volume));
                }

                var targetPrice = Trace.CalculateTargetPrice(candles, @event.HitPrice);

                await this._trendAnalysisIntegrationEventService.PublishThroughEventBusAsync(new NextTargetPriceGeneratedIntegrationEvent(
                    @event.TraceId,
                    @event.RoundtripId,
                    targetPrice
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: TargetPriceCandleDataCreatedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
