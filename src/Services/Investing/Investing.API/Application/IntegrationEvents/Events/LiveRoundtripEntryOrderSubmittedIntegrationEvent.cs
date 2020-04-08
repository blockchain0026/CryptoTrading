using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class LiveRoundtripEntryOrderSubmittedIntegrationEvent : IntegrationEvent
    {
        public LiveRoundtripEntryOrderSubmittedIntegrationEvent(string investmentId, string username, string roundtripId, int exchangeId, string baseCurrency, string quoteCurrency, decimal entryAmount, decimal entryPrice)
        {
            InvestmentId = investmentId;
            Username = username;
            RoundtripId = roundtripId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            EntryAmount = entryAmount;
            EntryPrice = entryPrice;
        }

        public string InvestmentId { get; private set; }
        public string Username { get; private set; }
        public string RoundtripId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal EntryAmount { get; private set; }
        public decimal EntryPrice { get; private set; }
    }
}
