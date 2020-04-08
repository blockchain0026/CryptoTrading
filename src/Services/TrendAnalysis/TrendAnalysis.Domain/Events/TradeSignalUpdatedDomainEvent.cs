using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Events
{
    public class TradeSignalUpdatedDomainEvent : INotification
    {
        public Trace Trace { get; private set; }

        public IEnumerable<Candle> Candles { get; private set; }

        public ITimeService TimeService { get; private set; }

        public TradeSignalUpdatedDomainEvent(Trace trace, IEnumerable<Candle> candles, ITimeService timeService)
        {
            Trace = trace;
            Candles = candles;
            TimeService = timeService;
        }

    }
}
