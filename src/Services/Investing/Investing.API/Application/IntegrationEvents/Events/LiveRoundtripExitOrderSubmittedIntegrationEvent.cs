using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class LiveRoundtripExitOrderSubmittedIntegrationEvent : IntegrationEvent
    {
        public LiveRoundtripExitOrderSubmittedIntegrationEvent(string investmentId, string username, string roundtripId, int exchangeId, string baseCurrency, string quoteCurrency, decimal exitAmount, decimal exitPrice)
        {
            InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            RoundtripId = roundtripId ?? throw new ArgumentNullException(nameof(roundtripId));
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            ExitAmount = exitAmount;
            ExitPrice = exitPrice;
        }

        public string InvestmentId { get; private set; }
        public string Username { get; private set; }
        public string RoundtripId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal ExitAmount { get; private set; }
        public decimal ExitPrice { get; private set; }
    }
}
