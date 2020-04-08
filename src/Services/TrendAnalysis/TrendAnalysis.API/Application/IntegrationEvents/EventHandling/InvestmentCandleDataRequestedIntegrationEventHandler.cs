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
    public class InvestmentCandleDataRequestedIntegrationEventHandler : IIntegrationEventHandler<InvestmentCandleDataRequestedIntegrationEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public InvestmentCandleDataRequestedIntegrationEventHandler(ITraceRepository traceRepository, ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(InvestmentCandleDataRequestedIntegrationEvent @event)
        {
            try
            {


                var runningTraces = await _traceRepository.GetByStatus(TraceStatus.Started);

                foreach (var trace in runningTraces)
                {
                    var market = new Market(@event.ExchangeId, @event.BaseCurrency.ToUpper(), @event.QuoteCurrency.ToUpper());
                    if (trace.Market.ExchangeId == @event.ExchangeId
                        && trace.Market.BaseCurrency == @event.BaseCurrency
                        && trace.Market.QuoteCurrency == @event.QuoteCurrency)
                    {
                        foreach (var strategy in trace.TradeStrategies)
                        {
                            if (strategy.GetIdealPeriod().Name == @event.CandlePeriod)
                            {
                                var minAmounts = strategy.Strategy.MinimumAmountOfCandles;
                                var period = strategy.GetIdealPeriod();
                                DateTime fromWithWarmingPeriod = (DateTime)trace.DateStarted;
                                var oneCandleMinutes = CandlePeriodService.GetOneCandleMinutesByPeriod(period);
                                var currentTime = new RealTimeService().GetCurrentDateTime();
                                var to = currentTime.AddMinutes(-oneCandleMinutes);
                                fromWithWarmingPeriod = fromWithWarmingPeriod.AddMinutes(-oneCandleMinutes * (minAmounts + 1));

                                await this._trendAnalysisIntegrationEventService
                                    .PublishThroughEventBusAsync(new TraceDataRequestedIntegrationEvent(
                                        trace.TraceId,
                                        strategy.StrategyId,
                                        trace.Market.ExchangeId,
                                        trace.Market.BaseCurrency,
                                        trace.Market.QuoteCurrency,
                                        strategy.GetIdealPeriod().Name,
                                        fromWithWarmingPeriod,
                                        to
                                        ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: CandleChartUpdatedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
