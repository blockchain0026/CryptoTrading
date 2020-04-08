using CryptoTrading.Services.Investing.API.Application.IntegrationEvents;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.API.Extensions;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentSettled
{
    public class InvestmentSettledDomainEventHandler : INotificationHandler<InvestmentSettledDomainEvent>
    {
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public InvestmentSettledDomainEventHandler(IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(InvestmentSettledDomainEvent investmentSettledDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                var investment = investmentSettledDomainEvent.Investment;

                /*DateTime started;
                DateTime closed;
                if(investment.InvestmentType.Id==InvestmentType.Backtesting.Id)
                {
                    started = (DateTime)investment.DateStarted;
                    closed = (DateTime)investment.DateClosed;
                }*/




                var @event = new InvestmentSettledIntegrationEvent(
                    investment.InvestmentId,
                    investment.Market.ExchangeId,
                    investment.Market.BaseCurrency,
                    investment.Market.QuoteCurrency
                    );

                await this._investingIntegrationEventService.PublishThroughEventBusAsync(@event);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentSettledDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
