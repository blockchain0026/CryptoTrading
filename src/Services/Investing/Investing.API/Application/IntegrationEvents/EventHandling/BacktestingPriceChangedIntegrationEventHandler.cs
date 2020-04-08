using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class BacktestingPriceChangedIntegrationEventHandler : IIntegrationEventHandler<BacktestingPriceChangedIntegrationEvent>
    {
        private readonly IRoundtripRepository _roundtripRepository;

        public BacktestingPriceChangedIntegrationEventHandler(IRoundtripRepository roundtripRepository)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
        }

        public async Task Handle(BacktestingPriceChangedIntegrationEvent @event)
        {
            IEnumerable<Roundtrip> roundtrips = null;
            Roundtrip roundtrip = null;

            try
            {

                roundtrips = await this._roundtripRepository.GetByInvestmentId(@event.InvestmentId);

                roundtrip = roundtrips.Where(r => r.GetStatus().Id == RoundtripStatus.Entry.Id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle IntegrationEvent Event: BacktestingPriceChangedIntegrationEventHandler.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }

            try
            {
                if (roundtrip != null)
                {
                    roundtrip.PriceChanged(@event.LowestPrice, @event.BacktestingCurrentTime, @event.HighestPrice, @event.LowestPrice, @event.TargetPrice);
                    this._roundtripRepository.Update(roundtrip);


                    await _roundtripRepository.UnitOfWork
                        .SaveEntitiesAsync();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle IntegrationEvent Event: BacktestingPriceChangedIntegrationEventHandler.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
