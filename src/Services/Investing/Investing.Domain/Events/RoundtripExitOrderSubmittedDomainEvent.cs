using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class RoundtripExitOrderSubmittedDomainEvent : INotification
    {
        public RoundtripExitOrderSubmittedDomainEvent(Roundtrip roundtrip, string investmentId, string roundtripId, RoundtripStatus roundtripStatus, Market market, decimal exitAmount, decimal exitPrice, DateTime adviceCreationDate)
        {
            Roundtrip = roundtrip;
            InvestmentId = investmentId;
            RoundtripId = roundtripId;
            RoundtripStatus = roundtripStatus;
            Market = market;
            ExitAmount = exitAmount;
            ExitPrice = exitPrice;
            AdviceCreationDate = adviceCreationDate;
        }

        public Roundtrip Roundtrip { get; private set; }
        public string InvestmentId { get; private set; }
        public string RoundtripId { get; private set; }
        public RoundtripStatus RoundtripStatus { get; private set; }
        public Market Market { get; private set; }
        public decimal ExitAmount { get; private set; }
        public decimal ExitPrice { get; private set; }
        public DateTime AdviceCreationDate { get; private set; }

    }
}
