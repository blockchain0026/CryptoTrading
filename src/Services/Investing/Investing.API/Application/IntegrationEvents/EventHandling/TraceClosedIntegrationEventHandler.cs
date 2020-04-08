using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class TraceClosedIntegrationEventHandler : IIntegrationEventHandler<TraceClosedIntegrationEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;

        public TraceClosedIntegrationEventHandler(IInvestmentRepository investmentRepository)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
        }

        public async Task Handle(TraceClosedIntegrationEvent @event)
        {
            try
            {
                var investment = await this._investmentRepository.GetByTrace(new Trace(@event.TraceId));
                investment.Close(forceClose: true);

                this._investmentRepository.Update(investment);

                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle IntegrationEvent Event: TraceClosedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
