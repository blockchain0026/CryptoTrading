using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class RoundtripEntryOrderSubmittedIntegrationEvent:IntegrationEvent
    {
        public RoundtripEntryOrderSubmittedIntegrationEvent(string investmentId, string roundtripId, int exchangeId, string baseCurrency, string quoteCurrency, decimal entryAmount, decimal entryPrice, DateTime adviceCreationDate)
        {
            InvestmentId = investmentId;
            RoundtripId = roundtripId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            EntryAmount = entryAmount;
            EntryPrice = entryPrice;
            AdviceCreationDate = adviceCreationDate;
        }

        public string InvestmentId { get; private set; }
        public string RoundtripId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal EntryAmount { get; private set; }
        public decimal EntryPrice { get; private set; }
        public DateTime AdviceCreationDate { get; private set; }
    }
}
