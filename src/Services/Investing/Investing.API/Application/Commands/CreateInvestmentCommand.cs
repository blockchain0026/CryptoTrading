using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Commands
{
    public class CreateInvestmentCommand : IRequest<bool>
    {
        public CreateInvestmentCommand(string investmentType, int exchangeId, string baseCurrency, string quoteCurrency, string username)
        {
            InvestmentType = investmentType;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Username = username;
        }

        public string InvestmentType { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public string Username { get; private set; }
    }
}
