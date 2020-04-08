using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class SettleInvestmentCommand:IRequest<bool>
    {
        public SettleInvestmentCommand(string investmentId)
        {
            InvestmentId = investmentId;
        }

        public string InvestmentId { get; private set; }
    }
}
