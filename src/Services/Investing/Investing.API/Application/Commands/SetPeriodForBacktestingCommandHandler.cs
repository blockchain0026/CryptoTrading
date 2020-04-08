using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Extensions;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class SetPeriodForBacktestingCommandHandler : IRequestHandler<SetPeriodForBacktestingCommand, bool>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus;

        public SetPeriodForBacktestingCommandHandler(IInvestmentRepository investmentRepository, IMediator mediator, IEventBus eventBus)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> Handle(SetPeriodForBacktestingCommand request, CancellationToken cancellationToken)
        {
            //Set Test Period

            try
            {
                var existingInvestment = await this._investmentRepository.GetByInvestmentId(request.InvestmentId);

                if (existingInvestment == null)
                {
                    throw new KeyNotFoundException($"Investment with Id {request.InvestmentId} not found.");
                }

                existingInvestment.SetPeriodForBacktesting(
                    request.From.ToDateTime(),
                    request.To.ToDateTime()
                    );

                this._investmentRepository.Update(existingInvestment);


                await _investmentRepository.UnitOfWork
                    .SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Command: SetPeriodForBacktestingCommand.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);

                return false;
            }


            return true;
        }
    }
}
