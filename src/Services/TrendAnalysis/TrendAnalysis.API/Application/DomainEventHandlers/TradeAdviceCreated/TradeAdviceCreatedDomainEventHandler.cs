using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.DomainEventHandlers.TradeAdviceCreated
{
    public class TradeAdviceCreatedDomainEventHandler : INotificationHandler<TradeAdviceCreatedDomainEvent>
    {
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public TradeAdviceCreatedDomainEventHandler(ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(TradeAdviceCreatedDomainEvent tradeAdviceCreatedDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                var advice = tradeAdviceCreatedDomainEvent.TradeAdvice;

                var @event = new TradeAdviceCreatedIntegrationEvent(
                    advice.TraceId,
                    advice.DateCreated,
                    tradeAdviceCreatedDomainEvent.TradingSignalType.Name,
                    advice.TargetPrice,
                    advice.Price);

                await this._trendAnalysisIntegrationEventService.PublishThroughEventBusAsync(@event);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: TradeAdviceCreatedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
