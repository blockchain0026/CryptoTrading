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
    public class RoundtripExitIntegrationEventHandler : IIntegrationEventHandler<RoundtripExitIntegrationEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public RoundtripExitIntegrationEventHandler(IInvestmentRepository investmentRepository, IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(RoundtripExitIntegrationEvent @event)
        {
            try
            {
                var investment = await this._investmentRepository.GetByInvestmentId(@event.InvestmentId);
                if (investment == null)
                {
                    throw new KeyNotFoundException(nameof(@event.InvestmentId));
                }

                var roundtrip = await this._roundtripRepository.GetByRoundtripId(@event.RoundtripId);
                if (roundtrip == null)
                {
                    throw new KeyNotFoundException(nameof(@event.RoundtripId));
                }

                investment.RoundtripExit(roundtrip);
                
                this._investmentRepository.Update(investment);



                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: RoundtripExitIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
