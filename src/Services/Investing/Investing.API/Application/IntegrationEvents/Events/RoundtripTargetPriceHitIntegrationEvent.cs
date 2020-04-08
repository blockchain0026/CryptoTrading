using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class RoundtripTargetPriceHitIntegrationEvent : IntegrationEvent
    {
        public RoundtripTargetPriceHitIntegrationEvent(string roundtripId, string investmentId, int exchangeId, string baseCurrency, string quoteCurrency, decimal hitPrice)
        {
            RoundtripId = roundtripId ?? throw new ArgumentNullException(nameof(roundtripId));
            InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            HitPrice = hitPrice;
        }

        public string RoundtripId { get; private set; }
        public string InvestmentId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal HitPrice { get; private set; }
    }
}
