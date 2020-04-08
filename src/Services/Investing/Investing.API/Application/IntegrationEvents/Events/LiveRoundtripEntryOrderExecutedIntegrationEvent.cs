using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class LiveRoundtripEntryOrderExecutedIntegrationEvent : IntegrationEvent
    {
        public LiveRoundtripEntryOrderExecutedIntegrationEvent(string investmentId, string roundtripId, int exchangeId, string orderId, string baseCurrency, string quoteCurrency, decimal executedAmount, decimal executedPrice, decimal fee)
        {
            InvestmentId = investmentId;
            RoundtripId = roundtripId;
            ExchangeId = exchangeId;
            OrderId = orderId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            ExecutedAmount = executedAmount;
            ExecutedPrice = executedPrice;
            Fee = fee;
        }

        public string InvestmentId { get; private set; }
        public string RoundtripId { get; private set; }
        public int ExchangeId { get; private set; }
        public string OrderId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal ExecutedAmount { get; private set; }
        public decimal ExecutedPrice { get; private set; }
        public decimal Fee { get; private set; }

    }
}
