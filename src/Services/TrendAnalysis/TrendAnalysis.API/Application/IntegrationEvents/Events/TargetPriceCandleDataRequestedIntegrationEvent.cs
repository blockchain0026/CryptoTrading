using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class TargetPriceCandleDataRequestedIntegrationEvent : IntegrationEvent
    {
        public TargetPriceCandleDataRequestedIntegrationEvent(string traceId, string roundtripId, int exchangeId, string baseCurrency, string quoteCurrency, string candlePeriod, decimal hitPrice, DateTime from, DateTime to)
        {
            TraceId = traceId;
            RoundtripId = roundtripId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            CandlePeriod = candlePeriod;
            HitPrice = hitPrice;
            From = from;
            To = to;
        }

        public string TraceId { get; private set; }
        public string RoundtripId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public string CandlePeriod { get; private set; }
        public decimal HitPrice { get; private set; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
    }
}
