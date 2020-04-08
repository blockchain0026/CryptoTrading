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
    public class StartInvestmentCommandHandler : IRequestHandler<StartInvestmentCommand, bool>
    {
        private readonly IInvestmentRepository _investmentRepository;
        //private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;

        public StartInvestmentCommandHandler(IInvestmentRepository investmentRepository, IEventBus eventBus)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> Handle(StartInvestmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var investment = await this._investmentRepository.GetByInvestmentId(request.InvestmentId);

                investment.Start();

                _investmentRepository.Update(investment);



                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Command: StartInvestmentCommand.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);

                return false;
            }


            return true;
        }
    }

    public class StartInvestmentIdentifiedCommandHandler : IdentifiedCommandHandler<StartInvestmentCommand, bool>
    {
        public StartInvestmentIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;        // Ignore duplicate requests for creating order.
        }
    }
}
