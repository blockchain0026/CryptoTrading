using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.TrendAnalysis.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class PaperTradeDataCreatedIntegrationEvent : IntegrationEvent
    {
        public PaperTradeDataCreatedIntegrationEvent(string traceId, string strategyId, string candlePeriod, int exchangeId, string baseCurrency, string quoteCurrency, List<Candle> candles)
        {
            TraceId = traceId;
            StrategyId = strategyId;
            CandlePeriod = candlePeriod;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Candles = candles;
        }

        public string TraceId { get; private set; }
        public string StrategyId { get; private set; }
        public string CandlePeriod { get; private set; }
        public int ExchangeId { get; private set; }

        public string BaseCurrency { get; private set; }

        public string QuoteCurrency { get; private set; }

        public List<CryptoTrading.Services.TrendAnalysis.API.Application.Models.Candle> Candles { get; private set; }
    }
}
