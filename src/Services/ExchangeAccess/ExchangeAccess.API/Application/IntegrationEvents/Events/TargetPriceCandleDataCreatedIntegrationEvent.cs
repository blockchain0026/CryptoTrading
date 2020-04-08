using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.ExchangeAccess.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events
{
    public class TargetPriceCandleDataCreatedIntegrationEvent : IntegrationEvent
    {
        public TargetPriceCandleDataCreatedIntegrationEvent(string traceId, string roundtripId, string candlePeriod, int exchangeId, string baseCurrency, string quoteCurrency, decimal hitPrice, List<Candle> candles)
        {
            TraceId = traceId ?? throw new ArgumentNullException(nameof(traceId));
            RoundtripId = roundtripId ?? throw new ArgumentNullException(nameof(roundtripId));
            CandlePeriod = candlePeriod ?? throw new ArgumentNullException(nameof(candlePeriod));
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            HitPrice = hitPrice;
            Candles = candles ?? throw new ArgumentNullException(nameof(candles));
        }

        public string TraceId { get; private set; }
        public string RoundtripId { get; private set; }
        public string CandlePeriod { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal HitPrice { get; private set; }
        public List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle> Candles { get; private set; }
    }
}
