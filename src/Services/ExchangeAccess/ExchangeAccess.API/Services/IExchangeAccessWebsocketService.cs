using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Services
{
    public interface IExchangeAccessWebsocketService
    {
        Task ListenToMarketOrderBook(string marketId);
        Task ListenToMarketCandle(string candleChartId);
        Task ListenToMarketTicker(string marketId);
        Task ListenToAccountBalance(string balanceId);
    }
}
