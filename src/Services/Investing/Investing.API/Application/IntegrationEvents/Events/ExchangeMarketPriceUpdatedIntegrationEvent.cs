using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class ExchangeMarketPriceUpdatedIntegrationEvent : IntegrationEvent
    {
        public ExchangeMarketPriceUpdatedIntegrationEvent(int exchangeId, string baseCurrency, string quoteCurrency, decimal price)
        {
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Price = price;
        }

        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
    }
}
