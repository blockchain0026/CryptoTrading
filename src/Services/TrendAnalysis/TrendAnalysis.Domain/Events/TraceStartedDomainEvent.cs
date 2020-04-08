using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Events
{
    public class TraceStartedDomainEvent : INotification
    {
        public string TraceId { get; private set; }
        public TraceStatus TraceStatus { get; private set; }
        public CandlePeriod IdealCandlePeriod { get; private set; }
        public DateTime DateStarted { get; private set; }

        public TraceStartedDomainEvent(string traceId, CandlePeriod idealCandlePeriod, TraceStatus traceStatus, DateTime dateStarted)
        {
            TraceId = traceId ?? throw new ArgumentNullException(nameof(traceId));
            IdealCandlePeriod = idealCandlePeriod ?? throw new ArgumentNullException(nameof(idealCandlePeriod));
            TraceStatus = traceStatus ?? throw new ArgumentNullException(nameof(traceStatus));
            this.DateStarted = dateStarted;
        }
    }
}
