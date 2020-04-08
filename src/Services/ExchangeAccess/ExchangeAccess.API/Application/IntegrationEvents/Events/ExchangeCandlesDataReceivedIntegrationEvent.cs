using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.ExchangeAccess.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events
{
    public class ExchangeCandleDataReceivedIntegrationEvent : IntegrationEvent
    {
        public ExchangeCandleDataReceivedIntegrationEvent(int exchangeId, string candleChartId, string baseCurrency, string quoteCurrency, Candle candle)
        {
            ExchangeId = exchangeId;
            CandleChartId = candleChartId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Candle = candle;
        }

        public int ExchangeId { get; private set; }
        public string CandleChartId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public Candle Candle { get; private set; }

    }
}
