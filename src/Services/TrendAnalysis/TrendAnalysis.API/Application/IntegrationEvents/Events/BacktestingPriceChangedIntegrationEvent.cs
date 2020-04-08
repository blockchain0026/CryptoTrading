using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class BacktestingPriceChangedIntegrationEvent : IntegrationEvent
    {
        public BacktestingPriceChangedIntegrationEvent(string traceId, string investmentId, DateTime backtestingCurrentTime, decimal highestPrice, decimal lowestPrice, decimal targetPrice)
        {
            TraceId = traceId;
            InvestmentId = investmentId;
            BacktestingCurrentTime = backtestingCurrentTime;
            HighestPrice = highestPrice;
            LowestPrice = lowestPrice;
            TargetPrice = targetPrice;
        }

        public string TraceId { get; private set; }
        public string InvestmentId { get; private set; }
        public DateTime BacktestingCurrentTime { get; private set; }
        public decimal HighestPrice { get; private set; }
        public decimal LowestPrice { get; private set; }
        public decimal TargetPrice { get; private set; }
    }
}
