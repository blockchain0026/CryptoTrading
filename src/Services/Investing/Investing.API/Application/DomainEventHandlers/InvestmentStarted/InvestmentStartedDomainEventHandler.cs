using CryptoTrading.Services.Investing.API.Application.IntegrationEvents;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentStarted
{
    public class InvestmentStartedDomainEventHandler : INotificationHandler<InvestmentStartedDomainEvent>
    {
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public InvestmentStartedDomainEventHandler(IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(InvestmentStartedDomainEvent investmentStartedDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                var investment = investmentStartedDomainEvent.Investment;

                if (investment.InvestmentType.Id == InvestmentType.Backtesting.Id)
                {
                    var @event = new BacktestingInvestmentStartedIntegrationEvent(
                        investment.InvestmentId,
                        investment.Trace.TraceId,
                        investment.InvestmentType.Name,
                        investment.Market.ExchangeId,
                        investment.Market.BaseCurrency,
                        investment.Market.QuoteCurrency,
                        investment.DateStarted ?? throw new ArgumentNullException(nameof(investment.DateStarted)),
                        investment.DateClosed ?? throw new ArgumentNullException(nameof(investment.DateStarted)),
                        investment.InitialBalance
                        );

                    await this._investingIntegrationEventService.PublishThroughEventBusAsync(@event);
                }
                else if (investment.InvestmentType.Id == InvestmentType.Paper.Id)
                {
                    var @event = new PaperInvestmentStartedIntegrationEvent(
                        investment.InvestmentId,
                        investment.Trace.TraceId,
                        investment.InvestmentType.Name,
                        investment.Market.ExchangeId,
                        investment.Market.BaseCurrency,
                        investment.Market.QuoteCurrency,
                        investment.DateStarted ?? throw new ArgumentNullException(nameof(investment.DateStarted)),
                        investment.InitialBalance
                        );

                    await this._investingIntegrationEventService.PublishThroughEventBusAsync(@event);
                }
                else if (investment.InvestmentType.Id == InvestmentType.Live.Id)
                {
                    var @event = new LiveInvestmentStartedIntegrationEvent(
                        investment.InvestmentId,
                        investment.Trace.TraceId,
                        investment.InvestmentType.Name,
                        investment.Market.ExchangeId,
                        investment.Market.BaseCurrency,
                        investment.Market.QuoteCurrency,
                        investment.DateStarted ?? throw new ArgumentNullException(nameof(investment.DateStarted)),
                        investment.InitialBalance
                        );

                    await this._investingIntegrationEventService.PublishThroughEventBusAsync(@event);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentStartedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
