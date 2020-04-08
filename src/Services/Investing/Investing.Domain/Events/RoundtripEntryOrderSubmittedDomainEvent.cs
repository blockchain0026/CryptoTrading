using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class RoundtripEntryOrderSubmittedDomainEvent : INotification
    {
        public RoundtripEntryOrderSubmittedDomainEvent(string investmentId, string roundtripId, RoundtripStatus roundtripStatus, Market market, decimal entryAmount, decimal entryPrice, DateTime adviceCreationDate)
        {
            InvestmentId = investmentId;
            RoundtripId = roundtripId;
            RoundtripStatus = roundtripStatus;
            Market = market;
            EntryAmount = entryAmount;
            EntryPrice = entryPrice;
            AdviceCreationDate = adviceCreationDate;
        }

        public string InvestmentId { get; private set; }
        public string RoundtripId { get; private set; }
        public RoundtripStatus RoundtripStatus { get; private set; }
        public Market Market { get; private set; }
        public decimal EntryAmount { get; private set; }
        public decimal EntryPrice { get; private set; }
        public DateTime AdviceCreationDate { get; private set; }
    }
}
