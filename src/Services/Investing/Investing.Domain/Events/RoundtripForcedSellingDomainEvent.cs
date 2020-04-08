using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class RoundtripForcedSellingDomainEvent : INotification
    {
        public RoundtripForcedSellingDomainEvent(string roundtripId, RoundtripStatus roundtripStatus)
        {
            RoundtripId = roundtripId ?? throw new ArgumentNullException(nameof(roundtripId));
            RoundtripStatus = roundtripStatus ?? throw new ArgumentNullException(nameof(roundtripStatus));
        }

        public string RoundtripId { get; private set; }
        public RoundtripStatus RoundtripStatus { get; private set; }
    }
}
