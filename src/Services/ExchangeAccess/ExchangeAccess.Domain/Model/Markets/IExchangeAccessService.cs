using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public interface IExchangeAccessService
    {
        Task<OrderBook> GetMarketData(int exchangeId, string baseCurrency, string quoteCurrency);
        Task<IEnumerable<Candle>> GetCandlesData(int exchangeId, string baseCurrency, string quoteCurrency, CandlePeriod candlePeriod, DateTime from, DateTime to);
        IEnumerable<Exchange> GetAvailableExchanges();
        Task ListenToMarketOrderBook(string marketId);
        Task<string> CreateBuyOrder(Account account, int exchangeId, string baseCurrency, string quoteCurrnecy, decimal price, decimal quantity);
        Task<string> CreateSellOrder(Account account, int exchangeId, string baseCurrency, string quoteCurrnecy, decimal price, decimal quantity);
        Task<bool> CancelOrder(Account account, int exchangeId, string orderId, string baseCurrency = default(string), string quoteCurrency = default(string));
        Task ListenToMarketTicker(string candleChartId);
        Task<IEnumerable<Order>> GetOpenOrders(Account account, int exchangeId);
        Task<Order> GetOrder(Account account, int exchangeId, string orderId, string baseCurrency = default(string), string quoteCurrency = default(string));
        Task<IEnumerable<Order>> GetOrderHistory(Account account, int exchangeId, string baseCurrency = null, string quoteCurrency = null);
        Task<IEnumerable<Asset>> GetAllBalance(Account account, int exchangeId);
    }
}
