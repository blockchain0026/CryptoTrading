using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.EventHandling
{
    public class BacktestingInvestmentStartedIntegrationEventHandler : IIntegrationEventHandler<BacktestingInvestmentStartedIntegrationEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public BacktestingInvestmentStartedIntegrationEventHandler(ITraceRepository traceRepository, ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(BacktestingInvestmentStartedIntegrationEvent @event)
        {
            var trace = await this._traceRepository.GetByTraceIdAsync(@event.TraceId);

            if (trace == null)
            {
                return;
            }

            if (@event.InvestmentType.ToLower() == "backtesting")
            {
                try
                {
                    var timeService = new BacktestingTimeService();
                    timeService.SetCurrentDateTime(@event.DateStarted);

                    trace.StartTracing(timeService);

                    this._traceRepository.Update(trace);


                    await _traceRepository.UnitOfWork
                        .SaveChangesAsync();





                    var minAmountOfCandles = new Dictionary<string, int>();

                    var existingTrace = await this._traceRepository.GetByTraceIdAsync(@event.TraceId);

                    foreach (var strategy in existingTrace.TradeStrategies)
                    {
                        var minAmounts = strategy.Strategy.MinimumAmountOfCandles;
                        var period = strategy.GetIdealPeriod();

                        if (minAmountOfCandles.TryGetValue(period.Name, out int existingData))
                        {
                            if (minAmounts > existingData)
                            {
                                minAmountOfCandles[period.Name] = minAmounts;
                            }
                        }
                        else
                        {
                            minAmountOfCandles.Add(period.Name, minAmounts);
                        }
                    }

                    await this._trendAnalysisIntegrationEventService.PublishThroughEventBusAsync(new BacktestingTraceStartedIntegrationEvent(
                        trace.TraceId,
                        trace.Investment.InvestmentId,
                        trace.Market.ExchangeId,
                        trace.Market.BaseCurrency,
                        trace.Market.QuoteCurrency,
                        @event.DateStarted,
                        @event.DateClosed,
                        minAmountOfCandles
                        ));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Handle Integration Event: BacktestingInvestmentStartedIntegrationEvent.");
                    Console.WriteLine("Result: Failure.");
                    Console.WriteLine("Error Message: " + ex.Message);
                }
            }

        }
    }
}
