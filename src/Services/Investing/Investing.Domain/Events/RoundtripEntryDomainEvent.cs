using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class RoundtripEntryDomainEvent : INotification
    {
        public RoundtripEntryDomainEvent(string investmentId, string roundtripId, RoundtripStatus roundtripStatus, Market market, DateTime entryAt, Transaction transaction)
        {
            InvestmentId = investmentId;
            RoundtripId = roundtripId;
            RoundtripStatus = roundtripStatus;
            Market = market;
            EntryAt = entryAt;
            Transaction = transaction;
        }

        public string InvestmentId { get; private set; }
        public string RoundtripId { get; private set; }
        public RoundtripStatus RoundtripStatus { get; private set; }
        public Market Market { get; private set; }
        public DateTime EntryAt { get; private set; }
        public Transaction Transaction { get; private set; }

    }
}
