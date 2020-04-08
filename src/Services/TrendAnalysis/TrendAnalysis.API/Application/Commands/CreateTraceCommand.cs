using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.Commands
{
    public class CreateTraceCommand : IRequest<bool>
    {
        public CreateTraceCommand(string investmentId, int exchangeId, string baseCurrency, string quoteCurrency, string[] strategies)
        {
            InvestmentId = investmentId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Strategies = strategies;
        }

        [DataMember]
        public string InvestmentId { get; private set; }

        [DataMember]
        public int ExchangeId { get; private set; }

        [DataMember]
        public string BaseCurrency { get; private set; }

        [DataMember]
        public string QuoteCurrency { get; private set; }

        [DataMember]
        public string[] Strategies { get; private set; }

    }
}
