using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class TraceClosedIntegrationEvent : IntegrationEvent
    {
        public TraceClosedIntegrationEvent(string traceId, DateTime dateClosed)
        {
            TraceId = traceId;
            DateClosed = dateClosed;
        }

        public string TraceId { get; private set; }
        public DateTime DateClosed { get; private set; }
    }
}
