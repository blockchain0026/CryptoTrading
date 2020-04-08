using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class RoundtripForcedExitDomainEvent : INotification
    {
        public RoundtripForcedExitDomainEvent(Roundtrip roundtrip)
        {
            Roundtrip = roundtrip ?? throw new ArgumentNullException(nameof(roundtrip));
        }

        public Roundtrip Roundtrip { get; private set; }
    }
}
