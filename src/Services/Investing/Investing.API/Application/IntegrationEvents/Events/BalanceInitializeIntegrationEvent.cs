using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.Services.Investing.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events
{
    public class BalanceInitializeIntegrationEvent : IntegrationEvent
    {
        public BalanceInitializeIntegrationEvent(int exchangeId, string balanceId, string username, IEnumerable<Asset> assets)
        {
            ExchangeId = exchangeId;
            BalanceId = balanceId;
            Username = username;
            Assets = assets;
        }

        public int ExchangeId { get; private set; }
        public string BalanceId { get; private set; }
        public string Username { get; private set; }
        public IEnumerable<Models.Asset> Assets { get; private set; }
    }
}
