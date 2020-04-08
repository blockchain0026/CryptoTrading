using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents;
using CryptoTrading.Services.TrendAnalysis.Domain.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.DomainEventHandlers.TradeSignalUpdated
{
    public class TradeSignalUpdatedDomainEventHandler
                  : INotificationHandler<TradeSignalUpdatedDomainEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public TradeSignalUpdatedDomainEventHandler(
           ITraceRepository traceRepository,
           ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            this._traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            this._trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(TradeSignalUpdatedDomainEvent tradeSignalUpdatedDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                var trace = await this._traceRepository.GetByTraceIdAsync(tradeSignalUpdatedDomainEvent.Trace.TraceId);

                trace.BuildAdvice(
                    tradeSignalUpdatedDomainEvent.Candles,
                    tradeSignalUpdatedDomainEvent.Candles.Last().Close,
                    tradeSignalUpdatedDomainEvent.TimeService);

                this._traceRepository.Update(trace);
                await this._traceRepository.UnitOfWork.SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: TradeSignalUpdatedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }

        }
    }
}
