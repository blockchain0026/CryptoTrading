using CryptoTrading.Services.Investing.API.Application.IntegrationEvents;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.RoundtripEntryOrderSubmitted
{
    public class RoundtripEntryOrderSubmittedDomainEventHandler : INotificationHandler<RoundtripEntryOrderSubmittedDomainEvent>
    {
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public RoundtripEntryOrderSubmittedDomainEventHandler(IInvestmentRepository investmentRepository, IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {

            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(RoundtripEntryOrderSubmittedDomainEvent roundtripEntryOrderSubmittedDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                var @event = new RoundtripEntryOrderSubmittedIntegrationEvent(
                       roundtripEntryOrderSubmittedDomainEvent.InvestmentId,
                       roundtripEntryOrderSubmittedDomainEvent.RoundtripId,
                       roundtripEntryOrderSubmittedDomainEvent.Market.ExchangeId,
                       roundtripEntryOrderSubmittedDomainEvent.Market.BaseCurrency,
                       roundtripEntryOrderSubmittedDomainEvent.Market.QuoteCurrency,
                       roundtripEntryOrderSubmittedDomainEvent.EntryAmount,
                       roundtripEntryOrderSubmittedDomainEvent.EntryPrice,
                       roundtripEntryOrderSubmittedDomainEvent.AdviceCreationDate
                       );
                await this._investingIntegrationEventService.PublishThroughEventBusAsync(@event);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: RoundtripEntryOrderSubmittedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }

        }
    }
}
