using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class SetPeriodForBacktestingCommand : IRequest<bool>
    {
        public SetPeriodForBacktestingCommand(string investmentId, long from, long to)
        {
            InvestmentId = investmentId;
            From = from;
            To = to;
        }

        public string InvestmentId { get; private set; }
        public long From { get; private set; }
        public long To { get; private set; }

    }
}
