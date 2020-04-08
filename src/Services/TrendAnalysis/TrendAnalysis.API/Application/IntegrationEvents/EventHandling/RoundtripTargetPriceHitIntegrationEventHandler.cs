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
    public class RoundtripTargetPriceHitIntegrationEventHandler : IIntegrationEventHandler<RoundtripTargetPriceHitIntegrationEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public RoundtripTargetPriceHitIntegrationEventHandler(ITraceRepository traceRepository, ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(RoundtripTargetPriceHitIntegrationEvent @event)
        {
            try
            {
                var trace = await this._traceRepository.GetByInvestmentId(@event.InvestmentId);

                if (trace == null)
                {
                    return;
                }

                var idealPeriod = trace.IdealCandlePeriod;

                int minAmounts = 0;
                foreach (var strategy in trace.TradeStrategies)
                {
                    if (strategy.GetIdealPeriod().Name == idealPeriod)
                    {
                        if (strategy.Strategy.MinimumAmountOfCandles > minAmounts)
                        {
                            minAmounts = strategy.Strategy.MinimumAmountOfCandles;
                        }
                    }
                }


                var period = CandlePeriod.FromName(idealPeriod);
                DateTime fromWithWarmingPeriod = (DateTime)trace.DateStarted;
                var oneCandleMinutes = CandlePeriodService.GetOneCandleMinutesByPeriod(period);
                var currentTime = new RealTimeService().GetCurrentDateTime();
                var to = currentTime.AddMinutes(-oneCandleMinutes);
                fromWithWarmingPeriod = fromWithWarmingPeriod.AddMinutes(-oneCandleMinutes * (minAmounts + 1));


                await this._trendAnalysisIntegrationEventService
                    .PublishThroughEventBusAsync(new TargetPriceCandleDataRequestedIntegrationEvent(
                        trace.TraceId,
                        @event.RoundtripId,
                        trace.Market.ExchangeId,
                        trace.Market.BaseCurrency,
                        trace.Market.QuoteCurrency,
                        idealPeriod,
                        @event.HitPrice,
                        fromWithWarmingPeriod,
                        to
                        ));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: RoundtripTargetPriceHitIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }

        }
    }
}
