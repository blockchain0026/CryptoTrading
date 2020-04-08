using CryptoTrading.Services.Investing.API.Application.IntegrationEvents;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.RoundtripTargetPriceHit
{
    public class RoundtripTargetPriceHitDomainEventHandler : INotificationHandler<RoundtripTargetPriceHitDomainEvent>
    {
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public RoundtripTargetPriceHitDomainEventHandler(IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(RoundtripTargetPriceHitDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                /*var roundtrips = await this._roundtripRepository.GetByInvestmentId(notification.InvestmentId);

                if (roundtrips.Any())
                {
                    var roundtrip = roundtrips.Where(r => r.RoundtripNumber == notification.RoundtripNumber).SingleOrDefault();
                    if (roundtrip == null)
                    {
                        return;
                    }

                    var now = DateTime.UtcNow;
                    roundtrip.Exit(
                        notification.TargetPrice,
                        new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second));

                    this._roundtripRepository.Update(roundtrip);

                    await _roundtripRepository.UnitOfWork
                        .SaveEntitiesAsync();

                }*/

                await this._investingIntegrationEventService.PublishThroughEventBusAsync(new RoundtripTargetPriceHitIntegrationEvent(
                    notification.RoundtripId,
                    notification.InvestmentId,
                    notification.Market.ExchangeId,
                    notification.Market.BaseCurrency,
                    notification.Market.QuoteCurrency,
                    notification.TargetPrice
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: RoundtripTargetPriceHitDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
