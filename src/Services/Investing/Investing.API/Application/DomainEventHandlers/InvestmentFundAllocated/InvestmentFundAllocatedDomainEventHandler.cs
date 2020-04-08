using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentFundAllocated
{
    public class InvestmentFundAllocatedDomainEventHandler : INotificationHandler<InvestmentFundAllocatedDomainEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;

        public InvestmentFundAllocatedDomainEventHandler(IInvestmentRepository investmentRepository)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
        }

        public async Task Handle(InvestmentFundAllocatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var investment = await this._investmentRepository.GetByInvestmentId(notification.InvestmentId);

                if (investment != null)
                {
                    investment.Funded(notification.Fund);

                    _investmentRepository.Update(investment);

                    await _investmentRepository.UnitOfWork.SaveEntitiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentFundAllocatedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
