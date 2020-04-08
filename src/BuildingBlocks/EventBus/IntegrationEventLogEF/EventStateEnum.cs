using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.BuildingBlocks.EventBus.IntegrationEventLogEF
{
    public enum EventStateEnum
    {
        NotPublished = 0,
        Published = 1,
        PublishedFailed = 2
    }
}
