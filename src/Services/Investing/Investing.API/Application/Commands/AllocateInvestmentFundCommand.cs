using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class AllocateInvestmentFundCommand : IRequest<bool>
    {
        public AllocateInvestmentFundCommand(string fundId, string investmentId, decimal quantity)
        {
            FundId = fundId;
            InvestmentId = investmentId;
            Quantity = quantity;
        }

        public string FundId { get; private set; }
        public string InvestmentId { get; private set; }
        public decimal Quantity { get; private set; }
    }
}
