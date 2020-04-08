using Binance.Net;
using Binance.Net.Objects;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.DomainServices
{
    public class ExchangeAccessService : IExchangeAccessService
    {
        private readonly IMarketRepository _marketRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICandleChartRepository _candleChartRepository;


        public ExchangeAccessService(IMarketRepository marketRepository, IBalanceRepository balanceRepository, ICandleChartRepository candleChartRepository)
        {
            this._marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            this._balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            this._candleChartRepository = candleChartRepository ?? throw new ArgumentNullException(nameof(candleChartRepository));
        }


        public async Task<bool> CancelOrder(Account account, int exchangeId, string orderId, string baseCurrency = null, string quoteCurrency = null)
        {
            if (exchangeId == 0)
            {
                using (var client = new ExchangeBinanceAPI())
                {
                    client.PublicApiKey = account.ApiKey.ToSecureString();
                    client.PrivateApiKey = account.ApiSecret.ToSecureString();
                    await client.CancelOrderAsync(orderId, baseCurrency.ToUpper() + quoteCurrency.ToUpper());

                    var openOrders = await this.GetOpenOrders(account, exchangeId);

                    bool isSuccess = !openOrders.Any(o => o.OrderId == orderId);
                    return isSuccess;
                }
            }


            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }

        public async Task<string> CreateBuyOrder(Account account, int exchangeId, string baseCurrency, string quoteCurrnecy, decimal price, decimal quantity)
        {
            if (exchangeId == 0)
            {
                using (var client = new ExchangeBinanceAPI())
                {
                    client.PublicApiKey = account.ApiKey.ToSecureString();
                    client.PrivateApiKey = account.ApiSecret.ToSecureString();

                    var orderRequest = new ExchangeOrderRequest
                    {
                        Symbol = baseCurrency.ToUpper() + quoteCurrnecy.ToUpper(),
                        Price = price,
                        Amount = quantity,
                        IsBuy = true,
                        OrderType = ExchangeSharp.OrderType.Limit
                    };

                    var result = await client.PlaceOrderAsync(orderRequest);

                    return result.OrderId;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }

        public async Task<string> CreateSellOrder(Account account, int exchangeId, string baseCurrency, string quoteCurrnecy, decimal price, decimal quantity)
        {
            if (exchangeId == 0)
            {
                using (var client = new ExchangeBinanceAPI())
                {
                    client.PublicApiKey = account.ApiKey.ToSecureString();
                    client.PrivateApiKey = account.ApiSecret.ToSecureString();

                    var orderRequest = new ExchangeOrderRequest
                    {
                        Symbol = baseCurrency.ToUpper() + quoteCurrnecy.ToUpper(),
                        Price = price,
                        Amount = quantity,
                        IsBuy = false,
                        OrderType = ExchangeSharp.OrderType.Limit
                    };

                    var result = await client.PlaceOrderAsync(orderRequest);

                    return result.OrderId;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }

        public IEnumerable<Exchange> GetAvailableExchanges()
        {
            var exchanges = new List<Exchange>();

            exchanges.Add(new Exchange(0, "Binance"));

            return exchanges.AsEnumerable();
        }

        public async Task<OrderBook> GetMarketData(int exchangeId, string baseCurrency, string quoteCurrency)
        {
            if (exchangeId == 0)
            {
                using (var client = new ExchangeBinanceAPI())
                {
                    var result = await client.GetOrderBookAsync(baseCurrency.ToUpper() + quoteCurrency.ToUpper(), 50);


                    return result.ToOrderBook(baseCurrency, quoteCurrency);
                }
            }

            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }

        public async Task<IEnumerable<Candle>> GetCandlesData(int exchangeId, string baseCurrency, string quoteCurrency, CandlePeriod candlePeriod, DateTime from, DateTime to)
        {
            if (exchangeId == 0)
            {
                using (var client = new BinanceClient())
                {
                    var result = new List<CryptoExchange.Net.Objects.CallResult<BinanceKline[]>>();

                    if (candlePeriod.Id == CandlePeriod.OneMinute.Id)
                    {
                        var amounts = to.Subtract(from).TotalMinutes;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts > 999)
                        {
                            nextTo = nextFrom.AddMinutes(999);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                               baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                               this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                               nextFrom,
                               nextTo,
                               limit: 1000));

                            nextFrom = nextTo.AddMinutes(1);
                            nextTo = to.Subtract(nextFrom).TotalMinutes > 999 ? nextFrom.AddMinutes(999) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else if (candlePeriod.Id == CandlePeriod.FiveMinutes.Id)
                    {
                        var amounts = to.Subtract(from).TotalMinutes;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts / 5 > 999)
                        {
                            nextTo = nextFrom.AddMinutes(999 * 5);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                                baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                                this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                                nextFrom,
                                nextTo,
                                limit: 1000));

                            nextFrom = nextTo.AddMinutes(5);
                            nextTo = to.Subtract(nextFrom).TotalMinutes / 5 > 999 ? nextFrom.AddMinutes(999 * 5) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else if (candlePeriod.Id == CandlePeriod.FifteenMinutes.Id)
                    {
                        var amounts = to.Subtract(from).TotalMinutes;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts / 15 > 999)
                        {
                            nextTo = nextFrom.AddMinutes(999 * 15);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                           baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                           this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                           nextFrom,
                           nextTo,
                           limit: 1000));

                            nextFrom = nextTo.AddMinutes(15);
                            nextTo = to.Subtract(nextFrom).TotalMinutes / 15 > 999 ? nextFrom.AddMinutes(999 * 15) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else if (candlePeriod.Id == CandlePeriod.ThirtyMinutes.Id)
                    {
                        var amounts = to.Subtract(from).TotalMinutes;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts / 30 > 999)
                        {
                            nextTo = nextFrom.AddMinutes(999 * 30);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                           baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                           this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                           nextFrom,
                           nextTo,
                           limit: 1000));

                            nextFrom = nextTo.AddMinutes(30);
                            nextTo = to.Subtract(nextFrom).TotalMinutes / 30 > 999 ? nextFrom.AddMinutes(999 * 30) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else if (candlePeriod.Id == CandlePeriod.OneHour.Id)
                    {
                        var amounts = to.Subtract(from).TotalHours;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts > 999)
                        {
                            nextTo = nextFrom.AddHours(999);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                              baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                              this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                              nextFrom,
                              nextTo,
                              limit: 1000));

                            nextFrom = nextTo.AddHours(1);
                            nextTo = to.Subtract(nextFrom).TotalHours > 999 ? nextFrom.AddHours(999) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else if (candlePeriod.Id == CandlePeriod.TwoHours.Id)
                    {
                        var amounts = to.Subtract(from).TotalHours;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts / 2 > 999)
                        {
                            nextTo = nextFrom.AddHours(999 * 2);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                         baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                         this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                         nextFrom,
                         nextTo,
                         limit: 1000));

                            nextFrom = nextTo.AddHours(2);
                            nextTo = to.Subtract(nextFrom).TotalHours / 2 > 999 ? nextFrom.AddHours(999 * 2) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else if (candlePeriod.Id == CandlePeriod.FourHours.Id)
                    {
                        var amounts = to.Subtract(from).TotalHours;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts / 4 > 999)
                        {
                            nextTo = nextFrom.AddHours(999 * 4);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                             baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                             this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                             nextFrom,
                             nextTo,
                             limit: 1000));

                            nextFrom = nextTo.AddHours(4);
                            nextTo = to.Subtract(nextFrom).TotalHours / 4 > 999 ? nextFrom.AddHours(999 * 4) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else if (candlePeriod.Id == CandlePeriod.OneDay.Id)
                    {
                        var amounts = to.Subtract(from).TotalDays;
                        var nextFrom = from;
                        var nextTo = to;

                        if (amounts > 999)
                        {
                            nextTo = nextFrom.AddDays(999);
                        }

                        do
                        {
                            result.Add(await client.GetKlinesAsync(
                             baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                             this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                             nextFrom,
                             nextTo,
                             limit: 1000));

                            nextFrom = nextTo.AddDays(1);
                            nextTo = to.Subtract(nextFrom).TotalDays > 999 ? nextFrom.AddDays(999) : to;
                        }
                        while (nextFrom <= to);
                    }
                    else
                    {
                        result.Add(await client.GetKlinesAsync(
                                                 baseCurrency.ToUpper() + quoteCurrency.ToUpper(),
                                                 this.GetKLineIntervalFromCalndlePeriod(candlePeriod),
                                                 from,
                                                 to,
                                                 limit: 1000));
                    }



                    var candles = new List<Candle>();

                    foreach (var part in result)
                    {
                        foreach (var candle in part.Data)
                        {
                            candles.Add(new Candle(
                                candle.OpenTime,
                                candle.High,
                                candle.Low,
                                candle.Open,
                                candle.Close,
                                candle.Volume
                                ));
                        }
                    }


                    return candles.AsEnumerable();
                }
            }

            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }


        public async Task<IEnumerable<Order>> GetOpenOrders(Account account, int exchangeId)
        {
            if (exchangeId == 0)
            {
                using (var client = new ExchangeBinanceAPI())
                {
                    client.PublicApiKey = account.ApiKey.ToSecureString();
                    client.PrivateApiKey = account.ApiSecret.ToSecureString();

                    var result = await client.GetOpenOrderDetailsAsync();


                    return result.ToOpenOrders(exchangeId);
                }
            }

            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }

        public async Task<Order> GetOrder(Account account, int exchangeId, string orderId, string baseCurrency = null, string quoteCurrency = null)
        {
            if (exchangeId == 0)
            {
                if (baseCurrency == null || quoteCurrency == null)
                {
                    throw new ArgumentNullException("Base currency and quote currency should be provided when getting orders from Binance exchange.");
                }
                using (var client = new BinanceClient())
                {
                    //Get fees and created date.
                    decimal fees = 0;
                    DateTime dateCreated;
                    using (var sharpClient = new ExchangeBinanceAPI())
                    {
                        sharpClient.PublicApiKey = account.ApiKey.ToSecureString();
                        sharpClient.PrivateApiKey = account.ApiSecret.ToSecureString();
                        var sharpResult = await sharpClient.GetOrderDetailsAsync(orderId, baseCurrency.ToUpper() + quoteCurrency.ToUpper());
                        fees = sharpResult.Fees;
                        dateCreated = sharpResult.OrderDate;
                    }


                    client.SetApiCredentials(account.ApiKey, account.ApiSecret);

                    var result = await client.QueryOrderAsync(baseCurrency.ToUpper() + quoteCurrency.ToUpper(), long.Parse(orderId));
                    var order = result.Data;

                    Domain.Model.Markets.OrderStatus orderStatus = this.GetStatusFromBiannceNet(order.Status);

                    //Calculate the average price.
                    var averageExecutedPrice = order.CummulativeQuoteQuantity / order.ExecutedQuantity;

                    return new Order(
                        exchangeId,
                        order.OrderId.ToString(),
                        order.Side == Binance.Net.Objects.OrderSide.Buy ? Domain.Model.Markets.OrderType.BUY_LIMIT : Domain.Model.Markets.OrderType.SELL_LIMIT,
                        orderStatus,
                        baseCurrency,
                        quoteCurrency,
                        averageExecutedPrice,
                        order.OriginalQuantity,
                        order.ExecutedQuantity,
                        fees,
                        dateCreated
                        );
                }

            }

            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }

        public async Task<IEnumerable<Order>> GetOrderHistory(Account account, int exchangeId, string baseCurrency = null, string quoteCurrency = null)
        {
            if (exchangeId == 0)
            {
                if (baseCurrency == null || quoteCurrency == null)
                {
                    throw new ArgumentNullException("Base currency and quote currency should be provided when getting order history from Binance exchange.");
                }

                using (var client = new BinanceClient())
                {
                    var result = await client.GetAllOrdersAsync(baseCurrency.ToUpper() + quoteCurrency.ToUpper());

                    var orders = new List<Order>();

                    foreach (var order in result.Data)
                    {
                        Domain.Model.Markets.OrderStatus orderStatus = this.GetStatusFromBiannceNet(order.Status);
                        orders.Add(new Order(
                        exchangeId,
                        order.OrderId.ToString(),
                        order.Side == Binance.Net.Objects.OrderSide.Buy ? Domain.Model.Markets.OrderType.BUY_LIMIT : Domain.Model.Markets.OrderType.SELL_LIMIT,
                        orderStatus,
                        baseCurrency,
                        quoteCurrency,
                        order.Price,
                        order.OriginalQuantity,
                        order.ExecutedQuantity,
                        order.ExecutedQuantity * 0.0025M,
                        order.Time
                        ));
                    }

                    return orders;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(exchangeId));
        }

        public async Task ListenToMarketOrderBook(string marketId)
        {
        }

        public async Task ListenToMarketTicker(string candleChartId)
        {
        }



        private Domain.Model.Markets.OrderStatus GetStatusFromBiannceNet(Binance.Net.Objects.OrderStatus orderStatus)
        {
            Domain.Model.Markets.OrderStatus result = null;

            switch (orderStatus)
            {
                case Binance.Net.Objects.OrderStatus.New:
                    result = Domain.Model.Markets.OrderStatus.New;
                    break;

                case Binance.Net.Objects.OrderStatus.PartiallyFilled:
                    result = Domain.Model.Markets.OrderStatus.PartiallyFilled;
                    break;
                case Binance.Net.Objects.OrderStatus.Filled:
                    result = Domain.Model.Markets.OrderStatus.Filled;
                    break;
                case Binance.Net.Objects.OrderStatus.Canceled:
                    result = Domain.Model.Markets.OrderStatus.Canceled;
                    break;
                case Binance.Net.Objects.OrderStatus.PendingCancel:
                    result = Domain.Model.Markets.OrderStatus.Canceled;
                    break;
                case Binance.Net.Objects.OrderStatus.Rejected:
                    result = Domain.Model.Markets.OrderStatus.Rejected;
                    break;
                case Binance.Net.Objects.OrderStatus.Expired:
                    result = Domain.Model.Markets.OrderStatus.Expired;
                    break;

            }

            return result;
        }

        private Domain.Model.CandleCharts.CandlePeriod GetPeriodFromBiannceNet(Binance.Net.Objects.KlineInterval klineInterval)
        {
            Domain.Model.CandleCharts.CandlePeriod result = null;

            switch (klineInterval)
            {
                case Binance.Net.Objects.KlineInterval.OneMinute:
                    result = CandlePeriod.OneMinute;
                    break;
                case Binance.Net.Objects.KlineInterval.FiveMinutes:
                    result = CandlePeriod.FiveMinutes;
                    break;
                case Binance.Net.Objects.KlineInterval.FiveteenMinutes:
                    result = CandlePeriod.FifteenMinutes;
                    break;
                case Binance.Net.Objects.KlineInterval.ThirtyMinutes:
                    result = CandlePeriod.ThirtyMinutes;
                    break;
                case Binance.Net.Objects.KlineInterval.OneHour:
                    result = CandlePeriod.OneHour;
                    break;
                case Binance.Net.Objects.KlineInterval.TwoHour:
                    result = CandlePeriod.TwoHours;
                    break;
                case Binance.Net.Objects.KlineInterval.FourHour:
                    result = CandlePeriod.FourHours;
                    break;
                case Binance.Net.Objects.KlineInterval.OneDay:
                    result = CandlePeriod.OneDay;
                    break;
                case Binance.Net.Objects.KlineInterval.OneWeek:
                    result = CandlePeriod.OneWeek;
                    break;

            }

            return result;
        }

        private Binance.Net.Objects.KlineInterval GetKLineIntervalFromCalndlePeriod(Domain.Model.CandleCharts.CandlePeriod candlePeriod)
        {
            Binance.Net.Objects.KlineInterval result;
            var id = candlePeriod.Id;

            if (id == CandlePeriod.OneMinute.Id)
                result = Binance.Net.Objects.KlineInterval.OneMinute;
            else if (id == CandlePeriod.FiveMinutes.Id)
                result = Binance.Net.Objects.KlineInterval.FiveMinutes;
            else if (id == CandlePeriod.FifteenMinutes.Id)
                result = Binance.Net.Objects.KlineInterval.FiveteenMinutes;
            else if (id == CandlePeriod.ThirtyMinutes.Id)
                result = Binance.Net.Objects.KlineInterval.ThirtyMinutes;
            else if (id == CandlePeriod.OneHour.Id)
                result = Binance.Net.Objects.KlineInterval.OneHour;
            else if (id == CandlePeriod.TwoHours.Id)
                result = Binance.Net.Objects.KlineInterval.TwoHour;
            else if (id == CandlePeriod.FourHours.Id)
                result = Binance.Net.Objects.KlineInterval.FourHour;
            else if (id == CandlePeriod.OneDay.Id)
                result = Binance.Net.Objects.KlineInterval.OneDay;
            else if (id == CandlePeriod.OneWeek.Id)
                result = Binance.Net.Objects.KlineInterval.OneWeek;
            else
                throw new ArgumentOutOfRangeException(nameof(candlePeriod));

            return result;
        }

        public async Task<IEnumerable<Asset>> GetAllBalance(Account account, int exchangeId)
        {

            using (var client = new BinanceClient())
            {
                var assets = new List<Asset>();

                client.SetApiCredentials(account.ApiKey, account.ApiSecret);

                //For debuging the gap between local and server time.
                var localTime = DateTime.UtcNow;
                var serverTime = await client.GetServerTimeAsync();

                var accountInfo = await client.GetAccountInfoAsync();
                var balances = accountInfo.Data.Balances;

                foreach (var asset in balances)
                {
                    if (asset.Total > 0)
                    {
                        assets.Add(new Asset(asset.Asset, asset.Free, asset.Locked));
                    }
                }

                return assets;
            }
        }
    }
}
