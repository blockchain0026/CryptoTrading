using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.TrendAnalysis.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class TargetPriceCandleDataCreatedIntegrationEvent : IntegrationEvent
    {
        public TargetPriceCandleDataCreatedIntegrationEvent(string traceId, string roundtripId, string candlePeriod, int exchangeId, string baseCurrency, string quoteCurrency, decimal hitPrice, List<Candle> candles)
        {
            TraceId = traceId;
            RoundtripId = roundtripId;
            CandlePeriod = candlePeriod;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            HitPrice = hitPrice;
            Candles = candles;
        }

        public string TraceId { get; private set; }
        public string RoundtripId { get; private set; }
        public string CandlePeriod { get; private set; }
        public int ExchangeId { get; private set; }

        public string BaseCurrency { get; private set; }

        public string QuoteCurrency { get; private set; }
        public decimal HitPrice { get; private set; }
        public List<CryptoTrading.Services.TrendAnalysis.API.Application.Models.Candle> Candles { get; private set; }
    }
}
