using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class PaperTraceStartedIntegrationEvent : IntegrationEvent
    {
        public PaperTraceStartedIntegrationEvent(string traceId, string investmentId, int exchangeId, string baseCurrency, string quoteCurrency, DateTime from, IDictionary<string, int> minAmountOfCandles)
        {
            TraceId = traceId;
            InvestmentId = investmentId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            From = from;
            MinAmountOfCandles = minAmountOfCandles;
        }

        public string TraceId { get; private set; }
        public string InvestmentId { get; private set; }

        public int ExchangeId { get; private set; }

        public string BaseCurrency { get; private set; }

        public string QuoteCurrency { get; private set; }
        public DateTime From { get; private set; }
        public IDictionary<string, int> MinAmountOfCandles { get; private set; }
    }
}
