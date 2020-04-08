using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.ExchangeAccess.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events
{
    public class ExchangeBalanceDataReceivedIntegrationEvent : IntegrationEvent
    {
        public ExchangeBalanceDataReceivedIntegrationEvent(int exchangeId, string balanceId, IEnumerable<Asset> assets)
        {
            ExchangeId = exchangeId;
            BalanceId = balanceId ?? throw new ArgumentNullException(nameof(balanceId));
            Assets = assets ?? throw new ArgumentNullException(nameof(assets));
        }

        public int ExchangeId { get; private set; }
        public string BalanceId { get; private set; }
        public IEnumerable<Models.Asset> Assets { get; private set; }
    }
}
