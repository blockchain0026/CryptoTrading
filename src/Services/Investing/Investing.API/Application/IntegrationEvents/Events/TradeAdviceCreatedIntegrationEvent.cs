using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class TradeAdviceCreatedIntegrationEvent : IntegrationEvent
    {
        public TradeAdviceCreatedIntegrationEvent(string traceId, DateTime dateCreated, string tradingSignalType, decimal targetPrice, decimal price)
        {
            TraceId = traceId;
            DateCreated = dateCreated;
            TradingSignalType = tradingSignalType;
            TargetPrice = targetPrice;
            Price = price;
        }

        public string TraceId { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string TradingSignalType { get; private set; }
        public decimal TargetPrice { get; private set; }
        public decimal Price { get; private set; }
    }
}
