using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.EventHandling
{
    public class PaperTradeDataCreatedIntegrationEventHandler : IIntegrationEventHandler<PaperTradeDataCreatedIntegrationEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public PaperTradeDataCreatedIntegrationEventHandler(ITraceRepository traceRepository, ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(PaperTradeDataCreatedIntegrationEvent @event)
        {
            try
            {
                var trace = await this._traceRepository.GetByTraceIdAsync(@event.TraceId);
                if (trace == null)
                {
                    return;
                }

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

                trace.CandleUpdated(candles, CandlePeriod.FromName(@event.CandlePeriod), new RealTimeService(), new IndicatorService(), @event.StrategyId);

                _traceRepository.Update(trace);


                await _traceRepository.UnitOfWork
                    .SaveEntitiesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integraion Event: PaperTradeDataCreatedIntegrationEvent \n" +
                    "Result: Failure. \n" +
                    "Error Message: " + ex.Message);
            }
        }
    }
}
