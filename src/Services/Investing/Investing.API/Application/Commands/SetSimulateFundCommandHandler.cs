using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class SetSimulateFundCommandHandler : IRequestHandler<SetSimulateFundCommand, bool>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IEventBus _eventBus;

        public SetSimulateFundCommandHandler(IInvestmentRepository investmentRepository, IEventBus eventBus)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> Handle(SetSimulateFundCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingInvestment = await this._investmentRepository.GetByInvestmentId(request.InvestmentId);

                if (existingInvestment == null)
                {
                    throw new KeyNotFoundException($"Investment with Id {request.InvestmentId} not found.");
                }

                existingInvestment.SetSimulateFund(request.Quantity);

                this._investmentRepository.Update(existingInvestment);


                await _investmentRepository.UnitOfWork
                    .SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Command: SetSimulateFundCommand.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);

                return false;
            }


            return true;
        }
    }

}
