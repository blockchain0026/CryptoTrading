using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class TraceCreatedIntegrationEvent : IntegrationEvent
    {
        public TraceCreatedIntegrationEvent(string traceId, string investmentId, int exchangeId, string baseCurrency, string quoteCurrency)
        {
            TraceId = traceId;
            InvestmentId = investmentId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
        }

        public string TraceId { get; private set; }
        public string InvestmentId { get; private set; }

        public int ExchangeId { get; private set; }

        public string BaseCurrency { get; private set; }

        public string QuoteCurrency { get; private set; }



    }
}
