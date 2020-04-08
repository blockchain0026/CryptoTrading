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

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.RoundtripExitOrderSubmitted
{

    public class RoundtripExitOrderSubmittedDomainEventHandler : INotificationHandler<RoundtripExitOrderSubmittedDomainEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public RoundtripExitOrderSubmittedDomainEventHandler(IInvestmentRepository investmentRepository, IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(RoundtripExitOrderSubmittedDomainEvent roundtripExitOrderSubmittedDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                var investment = await this._investmentRepository.GetByInvestmentId(roundtripExitOrderSubmittedDomainEvent.InvestmentId);

                if (investment == null)
                {
                    throw new KeyNotFoundException(nameof(roundtripExitOrderSubmittedDomainEvent.InvestmentId));
                }


                if (investment.InvestmentType.Id == InvestmentType.Live.Id)
                {
                    var slipExitPrice = roundtripExitOrderSubmittedDomainEvent.ExitPrice * 0.99M;
                    var exitAmount = roundtripExitOrderSubmittedDomainEvent.ExitAmount.ToString("N6");
                    var @event = new LiveRoundtripExitOrderSubmittedIntegrationEvent(
                       investment.InvestmentId,
                       investment.Account.Username,
                       roundtripExitOrderSubmittedDomainEvent.RoundtripId,
                       roundtripExitOrderSubmittedDomainEvent.Market.ExchangeId,
                       roundtripExitOrderSubmittedDomainEvent.Market.BaseCurrency,
                       roundtripExitOrderSubmittedDomainEvent.Market.QuoteCurrency,
                       decimal.Parse(exitAmount),
                       slipExitPrice
                       );

                    await this._investingIntegrationEventService.PublishThroughEventBusAsync(@event);
                }
                else
                {
                    var roundtrip = await this._roundtripRepository.GetByRoundtripId(roundtripExitOrderSubmittedDomainEvent.RoundtripId);
                    //roundtrip.ClearDomainEvents();
                    if (roundtrip == null)
                    {
                        throw new KeyNotFoundException(nameof(roundtripExitOrderSubmittedDomainEvent.Roundtrip));
                    }

                    roundtrip.OrderFilled(roundtripExitOrderSubmittedDomainEvent.ExitAmount, roundtripExitOrderSubmittedDomainEvent.ExitPrice, roundtripExitOrderSubmittedDomainEvent.AdviceCreationDate);

                    this._roundtripRepository.Update(roundtrip);

                    if (investment.InvestmentType.Id == InvestmentType.Paper.Id)
                    {
                        await _roundtripRepository.UnitOfWork
                            .SaveEntitiesAsync();
                    }
                    else
                    {
                        await _roundtripRepository.UnitOfWork
                            .SaveChangesAsync();
                    }



                    await this._investingIntegrationEventService
                        .PublishThroughEventBusAsync(new RoundtripExitIntegrationEvent(
                            roundtrip.InvestmentId,
                            roundtrip.RoundtripId,
                            roundtrip.ExitBalance ?? throw new ArgumentNullException(nameof(roundtrip.ExitBalance)),
                            roundtrip.ExitAt ?? throw new ArgumentNullException(nameof(roundtrip.ExitAt)),
                            roundtrip.Transaction.SellPrice ?? throw new ArgumentNullException(nameof(roundtrip.Transaction.SellPrice))
                            ));
                }
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
