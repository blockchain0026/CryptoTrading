using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.DomainEventHandlers.TraceClosed
{
    public class TraceClosedDomainEventHandler : INotificationHandler<TraceClosedDomainEvent>
    {
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public TraceClosedDomainEventHandler(ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(TraceClosedDomainEvent traceClosedDomainEvent, CancellationToken cancellationToken)
        {
            var @event = new TraceClosedIntegrationEvent(traceClosedDomainEvent.TraceId, traceClosedDomainEvent.DateClosed);
            await this._trendAnalysisIntegrationEventService.PublishThroughEventBusAsync(@event);
        }
    }
}
