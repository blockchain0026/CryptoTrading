using CryptoTrading.Services.Investing.API.Application.IntegrationEvents;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentReady
{
    public class InvestmentReadyDomainEventHandler : INotificationHandler<InvestmentReadyDomainEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public InvestmentReadyDomainEventHandler(IInvestmentRepository investmentRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(InvestmentReadyDomainEvent investmentReadyDomainEvent, CancellationToken cancellationToken)
        {
           /* var investment = investmentReadyDomainEvent.Investment;

            if (investmentReadyDomainEvent.InvestmentStatus.Id == InvestmentStatus.Ready.Id)
            {
                investment.Start();
            }

            _investmentRepository.Update(investment);

            try
            {
                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentReadyDomainEventHandler.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }*/
        }
    }
}
