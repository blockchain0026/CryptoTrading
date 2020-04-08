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
    public class AllocateInvestmentFundCommandHandler : IRequestHandler<AllocateInvestmentFundCommand, bool>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IFundRepository _fundRepository;

        public AllocateInvestmentFundCommandHandler(IInvestmentRepository investmentRepository, IFundRepository fundRepository)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _fundRepository = fundRepository ?? throw new ArgumentNullException(nameof(fundRepository));
        }

        public async Task<bool> Handle(AllocateInvestmentFundCommand request, CancellationToken cancellationToken)
        {
            var investment = await this._investmentRepository.GetByInvestmentId(request.InvestmentId);
            var fund = await this._fundRepository.GetByFundId(request.FundId);


            if (investment == null || fund == null)
            {
                return false;
            }

            fund.AllocateFundsFor(investment, request.Quantity);

            this._fundRepository.Update(fund);
            await this._fundRepository.UnitOfWork.SaveEntitiesAsync();

            return true;
        }
    }

    public class AllocateInvestmentFundIdentifiedCommandHandler : IdentifiedCommandHandler<AllocateInvestmentFundCommand, bool>
    {
        public AllocateInvestmentFundIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;        // Ignore duplicate requests for creating order.
        }
    }
}
