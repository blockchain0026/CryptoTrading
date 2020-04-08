using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class SetSimulateFundCommand : IRequest<bool>
    {
        public SetSimulateFundCommand(string investmentId, decimal quantity)
        {
            InvestmentId = investmentId;
            Quantity = quantity;
        }

        public string InvestmentId { get; private set; }
        public decimal Quantity { get; private set; }
    }
}
