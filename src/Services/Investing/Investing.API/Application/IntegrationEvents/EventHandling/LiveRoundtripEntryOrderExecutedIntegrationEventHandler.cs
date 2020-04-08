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
    public class LiveRoundtripEntryOrderExecutedIntegrationEventHandler : IIntegrationEventHandler<LiveRoundtripEntryOrderExecutedIntegrationEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public LiveRoundtripEntryOrderExecutedIntegrationEventHandler(IInvestmentRepository investmentRepository, IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(LiveRoundtripEntryOrderExecutedIntegrationEvent @event)
        {
            try
            {

                var roundtrip = await this._roundtripRepository.GetByRoundtripId(@event.RoundtripId);
                if (roundtrip == null)
                {
                    throw new KeyNotFoundException(nameof(@event.RoundtripId));
                }

                //Binance's transaction fee.
                var feePercent = 0.001M;
                roundtrip.OrderFilled(
                    @event.ExecutedAmount,
                    @event.ExecutedPrice,
                    @event.CreationDate,
                    feePercent);

                this._roundtripRepository.Update(roundtrip);
                await _roundtripRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: LiveRoundtripEntryOrderExecutedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
