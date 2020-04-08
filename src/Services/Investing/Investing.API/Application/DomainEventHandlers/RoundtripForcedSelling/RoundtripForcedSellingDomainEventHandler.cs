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

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.RoundtripForcedSelling
{
    public class RoundtripForcedSellingDomainEventHandler : INotificationHandler<RoundtripForcedSellingDomainEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public RoundtripForcedSellingDomainEventHandler(IInvestmentRepository investmentRepository, IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(RoundtripForcedSellingDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var roundtrip = await this._roundtripRepository.GetByRoundtripId(notification.RoundtripId);

                var investment = await this._investmentRepository.GetByInvestmentId(roundtrip.InvestmentId);

                if (investment == null)
                {
                    throw new KeyNotFoundException(nameof(investment.InvestmentId));
                }


                if (investment.InvestmentType.Id == InvestmentType.Live.Id)
                {
                    /*var @event = new LiveRoundtripExitOrderSubmittedIntegrationEvent(
                       investment.InvestmentId,
                       
                       roundtrip.RoundtripId,
                       roundtrip.Market.ExchangeId,
                       roundtrip.Market.BaseCurrency,
                       roundtrip.Market.QuoteCurrency,
                       0,
                       0
                       );

                    await this._investingIntegrationEventService.PublishThroughEventBusAsync(@event);*/
                }
                else
                {
                    //roundtrip.ClearDomainEvents();
                    if (roundtrip == null)
                    {
                        throw new KeyNotFoundException(nameof(notification.RoundtripId));
                    }

                    roundtrip.ForceSellingSuccess(
                        (decimal)roundtrip.Transaction.BuyAmount, (decimal)roundtrip.Transaction.BuyPrice, (DateTime)roundtrip.EntryAt);

                    this._roundtripRepository.Update(roundtrip);



                    await _roundtripRepository.UnitOfWork
                        .SaveChangesAsync();




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
