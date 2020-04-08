using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents
{
    public interface ITrendAnalysisIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
