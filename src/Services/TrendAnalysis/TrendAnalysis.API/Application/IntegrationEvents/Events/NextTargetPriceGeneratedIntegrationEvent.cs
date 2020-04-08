using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class NextTargetPriceGeneratedIntegrationEvent : IntegrationEvent
    {
        public NextTargetPriceGeneratedIntegrationEvent(string traceId, string roundtripId, decimal nextTargetPrice)
        {
            TraceId = traceId;
            RoundtripId = roundtripId;
            NextTargetPrice = nextTargetPrice;
        }

        public string TraceId { get; private set; }
        public string RoundtripId { get; private set; }
        public decimal NextTargetPrice { get; private set; }
    }
}
