using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class StartInvestmentCommand : IRequest<bool>
    {
        public StartInvestmentCommand(string investmentId)
        {
            InvestmentId = investmentId;
        }

        public string InvestmentId { get; private set; }
    }
}
