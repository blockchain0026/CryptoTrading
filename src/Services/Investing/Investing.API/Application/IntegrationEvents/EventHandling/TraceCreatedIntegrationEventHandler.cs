using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class TraceCreatedIntegrationEventHandler : IIntegrationEventHandler<TraceCreatedIntegrationEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public TraceCreatedIntegrationEventHandler(IInvestmentRepository investmentRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }


        public async Task Handle(TraceCreatedIntegrationEvent @event)
        {
            var existingInvestment = await this._investmentRepository.GetByInvestmentId(@event.InvestmentId);

            if(existingInvestment!=null)
            {
                existingInvestment.TraceArranged(new Trace(@event.TraceId));
            }

            try
            {
                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: BacktestingTraceCreatedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
