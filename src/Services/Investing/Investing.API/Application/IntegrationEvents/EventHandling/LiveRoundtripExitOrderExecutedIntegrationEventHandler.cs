using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class LiveRoundtripExitOrderExecutedIntegrationEventHandler : IIntegrationEventHandler<LiveRoundtripExitOrderExecutedIntegrationEvent>
    {
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public LiveRoundtripExitOrderExecutedIntegrationEventHandler(IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(LiveRoundtripExitOrderExecutedIntegrationEvent @event)
        {
            try
            {
                var roundtrip = await this._roundtripRepository.GetByRoundtripId(@event.RoundtripId);
                //roundtrip.ClearDomainEvents();
                if (roundtrip == null)
                {
                    throw new KeyNotFoundException(nameof(@event.RoundtripId));
                }

                //Binance's transaction fee.
                var feePercent = 0.001M;
                roundtrip.OrderFilled(@event.ExecutedAmount, @event.ExecutedPrice, @event.CreationDate,feePercent);

                this._roundtripRepository.Update(roundtrip);


                await _roundtripRepository.UnitOfWork
                    .SaveEntitiesAsync();


                await this._investingIntegrationEventService
                    .PublishThroughEventBusAsync(new RoundtripExitIntegrationEvent(
                        roundtrip.InvestmentId,
                        roundtrip.RoundtripId,
                        roundtrip.ExitBalance ?? throw new ArgumentNullException(nameof(roundtrip.ExitBalance)),
                        roundtrip.ExitAt ?? throw new ArgumentNullException(nameof(roundtrip.ExitAt)),
                        roundtrip.Transaction.SellPrice ?? throw new ArgumentNullException(nameof(roundtrip.Transaction.SellPrice))
                        ));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: LiveRoundtripExitOrderExecutedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message); 
            }
        }
    }
}
