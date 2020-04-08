using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class InvestmentCandleDataRequestedIntegrationEvent:IntegrationEvent
    {
        public InvestmentCandleDataRequestedIntegrationEvent(string candleChartId, int exchangeId, string baseCurrency, string quoteCurrency, string candlePeriod, DateTime timestamp, decimal high, decimal low, decimal open, decimal close, decimal volume)
        {
            CandleChartId = candleChartId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            CandlePeriod = candlePeriod;
            Timestamp = timestamp;
            High = high;
            Low = low;
            Open = open;
            Close = close;
            Volume = volume;
        }

        public string CandleChartId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public string CandlePeriod { get; private set; }
        public DateTime Timestamp { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }
        public decimal Open { get; private set; }
        public decimal Close { get; private set; }
        public decimal Volume { get; private set; }
    }
}
