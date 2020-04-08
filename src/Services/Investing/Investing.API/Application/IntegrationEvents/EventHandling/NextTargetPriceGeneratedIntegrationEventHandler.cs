using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class NextTargetPriceGeneratedIntegrationEventHandler : IIntegrationEventHandler<NextTargetPriceGeneratedIntegrationEvent>
    {
        private readonly IRoundtripRepository _roundtripRepository;

        public NextTargetPriceGeneratedIntegrationEventHandler(IRoundtripRepository roundtripRepository)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
        }

        public async Task Handle(NextTargetPriceGeneratedIntegrationEvent @event)
        {
            try
            {
                var roundtrip = await _roundtripRepository.GetByRoundtripId(@event.RoundtripId);

                if (roundtrip == null)
                {
                    return;
                }

                roundtrip.MoveTargetPrice(@event.NextTargetPrice);

                _roundtripRepository.Update(roundtrip);

                await _roundtripRepository.UnitOfWork.SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: NextTargetPriceGeneratedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
