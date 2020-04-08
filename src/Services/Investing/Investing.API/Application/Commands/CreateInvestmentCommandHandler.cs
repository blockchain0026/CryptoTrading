using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.Domain.Model.Funds;
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
    public class CreateInvestmentCommandHandler : IRequestHandler<CreateInvestmentCommand, bool>
    {
        private readonly IInvestmentRepository _investmentRepository;
        //private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus;

        public CreateInvestmentCommandHandler(IInvestmentRepository investmentRepository, IMediator mediator, IEventBus eventBus)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> Handle(CreateInvestmentCommand message, CancellationToken cancellationToken)
        {
            //Prepare a new investment.
            try
            {
                var investment = Investment.FromType(
                    InvestmentType.FromName(message.InvestmentType),
                    message.ExchangeId,
                    message.BaseCurrency,
                    message.QuoteCurrency,
                    new Account(message.Username)
                    );

                _investmentRepository.Add(investment);



                await _investmentRepository.UnitOfWork
                    .SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Command: CreateInvestmentCommand.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);

                return false;
            }


            return true;
        }
    }

    // Use for Idempotency in Command process
    public class CreateInvestmentIdentifiedCommandHandler : IdentifiedCommandHandler<CreateInvestmentCommand, bool>
    {
        public CreateInvestmentIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;        // Ignore duplicate requests for creating order.
        }
    }
}
