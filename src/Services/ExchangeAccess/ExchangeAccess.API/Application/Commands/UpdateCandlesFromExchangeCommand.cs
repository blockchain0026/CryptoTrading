using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.Commands
{

    public class UpdateCandlesFromExchangeCommand : IRequest<bool>
    {
        public UpdateCandlesFromExchangeCommand(int exchangeId, string baseCurrency, string quoteCurrency, string candlePeriod, long from, long to)
        {
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            From = from;
            To = to;
            CandlePeriod = candlePeriod;
        }

        [DataMember]
        public int ExchangeId { get; private set; }
        [DataMember]
        public string BaseCurrency { get; private set; }

        [DataMember]
        public string QuoteCurrency { get; private set; }

        [DataMember]
        public string CandlePeriod { get; private set; }

        [DataMember]
        public long From { get; private set; }

        [DataMember]
        public long To { get; private set; }

    }
}
