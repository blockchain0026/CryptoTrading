using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.Commands
{
    public class CreateMarketCommand : IRequest<bool>
    {
        public CreateMarketCommand(int exchangeId, string baseCurrency, string quoteCurrency)
        {
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
        }

        [DataMember]
        public int ExchangeId { get; private set; }
        [DataMember]
        public string BaseCurrency { get; private set; }

        [DataMember]
        public string QuoteCurrency { get; private set; }

    }
}
