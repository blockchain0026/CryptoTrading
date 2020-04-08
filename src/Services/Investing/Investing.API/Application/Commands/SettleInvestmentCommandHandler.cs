using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Infrastructure.Idempotency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class SettleInvestmentCommandHandler : IRequestHandler<SettleInvestmentCommand, bool>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IEventBus _eventBus;

        public SettleInvestmentCommandHandler(IInvestmentRepository investmentRepository, IEventBus eventBus)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }


        public async Task<bool> Handle(SettleInvestmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
            var existingInvestment = await this._investmentRepository.GetByInvestmentId(request.InvestmentId);

            if (existingInvestment == null)
            {
                throw new KeyNotFoundException($"Investment with Id {request.InvestmentId} not found.");
            }

            existingInvestment.Settle();

            this._investmentRepository.Update(existingInvestment);

            
                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Command: SettleInvestmentCommand.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
                return false;
            }


            return true;
        }
    }
    

    public class SettleInvestmentIdentifiedCommandHandler : IdentifiedCommandHandler<SettleInvestmentCommand, bool>
    {
        public SettleInvestmentIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;        // Ignore duplicate requests for creating order.
        }
    }
}
