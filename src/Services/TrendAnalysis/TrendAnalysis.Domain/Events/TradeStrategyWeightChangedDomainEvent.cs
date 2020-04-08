using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Events
{
    public class TradeStrategyWeightChangedDomainEvent : INotification
    {


        public string TraceId { get; private set; }
        public string StrategyId { get; private set; }
        public string StrategyName { get; private set; }
        public int Weight { get; private set; }

        public TradeStrategyWeightChangedDomainEvent(string traceId, string strategyId, string strategyName, int weight)
        {
            TraceId = traceId;
            StrategyId = strategyId;
            StrategyName = strategyName;
            Weight = weight;
        }

    }
}
