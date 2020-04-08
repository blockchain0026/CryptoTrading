using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class RoundtripExitIntegrationEvent : IntegrationEvent
    {
        public RoundtripExitIntegrationEvent(string investmentId, string roundtripId, decimal exitBalance, DateTime exitAt, decimal exitPrice)
        {
            InvestmentId = investmentId;
            RoundtripId = roundtripId;
            ExitBalance = exitBalance;
            ExitAt = exitAt;
            ExitPrice = exitPrice;
        }

        public string InvestmentId { get; private set; }
        public string RoundtripId { get; private set; }

        public decimal ExitBalance { get; private set; }
        public DateTime ExitAt { get; private set; }
        public decimal ExitPrice { get; private set; }
    }
}
