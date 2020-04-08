using Binance.Net.Objects;
using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events
{
    public class ExchangeOrderBookDataReceivedIntegrationEvent : IntegrationEvent
    {
        public int ExchangeId { get; private set; }
        public OrderBook OrderBook { get; private set; }

        public ExchangeOrderBookDataReceivedIntegrationEvent(int exchangeId, OrderBook orderBook)
        {
            ExchangeId = exchangeId;
            OrderBook = orderBook;
        }
    }
}
