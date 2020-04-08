using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class InvestmentSettledIntegrationEvent : IntegrationEvent
    {
        public InvestmentSettledIntegrationEvent(string investmentId, int exchangeId, string baseCurrency, string quoteCurrency)
        {
            InvestmentId = investmentId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
        }

        public string InvestmentId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
    }
}
