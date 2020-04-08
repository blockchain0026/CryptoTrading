using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class RoundtripTargetPriceHitDomainEvent : INotification
    {
        public RoundtripTargetPriceHitDomainEvent(string roundtripId, string investmentId, int roundtripNumber, decimal targetPrice, Market market)
        {
            RoundtripId = roundtripId ?? throw new ArgumentNullException(nameof(roundtripId));
            InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
            RoundtripNumber = roundtripNumber;
            TargetPrice = targetPrice;
            Market = market ?? throw new ArgumentNullException(nameof(market));
        }

        public string RoundtripId { get; private set; }
        public string InvestmentId { get; private set; }
        public int RoundtripNumber { get; private set; }
        public decimal TargetPrice { get; private set; }
        public Market Market { get; private set; }
    }
}
