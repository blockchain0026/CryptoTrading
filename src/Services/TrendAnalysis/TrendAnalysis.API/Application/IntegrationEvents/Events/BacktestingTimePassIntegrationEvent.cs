using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class BacktestingTimePassIntegrationEvent:IntegrationEvent
    {
        public BacktestingTimePassIntegrationEvent(DateTime backtestingCurrentTime, string traceId, string investmentId)
        {
            BacktestingCurrentTime = backtestingCurrentTime;
            TraceId = traceId;
            InvestmentId = investmentId;
        }

        public DateTime BacktestingCurrentTime { get; private set; }
        public string TraceId { get; private set; }
        public string InvestmentId { get; private set; }
    }
}
