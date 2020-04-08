using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.TrendAnalysis.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class BacktestingDataCreatedIntegrationEvent : IntegrationEvent
    {
        public BacktestingDataCreatedIntegrationEvent(string traceId, string investmentId, int exchangeId, string baseCurrency, string quoteCurrency, long from, long to, IDictionary<string, List<Candle>> candles)
        {
            TraceId = traceId;
            InvestmentId = investmentId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            From = from;
            To = to;
            Candles = candles;
        }

        public string TraceId { get; private set; }
        public string InvestmentId { get; private set; }

        public int ExchangeId { get; private set; }

        public string BaseCurrency { get; private set; }

        public string QuoteCurrency { get; private set; }

        public long From { get; private set; }

        public long To { get; private set; }

        public IDictionary<string, List<CryptoTrading.Services.TrendAnalysis.API.Application.Models.Candle>> Candles { get; private set; }

    }
}
