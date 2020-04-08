using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Events
{
    public class TraceCreatedDomainEvent : INotification
    {
        public Trace Trace { get; private set; }


        public TraceCreatedDomainEvent(Trace trace)
        {
            Trace = trace;
        }
    }
}
