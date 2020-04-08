using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Events
{
    public class TraceClosedDomainEvent : INotification
    {
        public string TraceId { get; private set; }
        public TraceStatus TraceStatus { get; private set; }
        public DateTime DateClosed { get; private set; }

        public TraceClosedDomainEvent(string traceId, TraceStatus traceStatus, DateTime dateClosed)
        {
            TraceId = traceId ?? throw new ArgumentNullException(nameof(traceId));
            TraceStatus = traceStatus ?? throw new ArgumentNullException(nameof(traceStatus));
            DateClosed = dateClosed;
        }
    }
}
