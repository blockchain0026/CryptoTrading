using Binance.Net;
using Binance.Net.Objects;
using Binance.WebSocket;
using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Services
{
    public class ExchangeAccessWebsocketService : IExchangeAccessWebsocketService
    {
        public delegate void ExchangeCallback(int exchangeId, string baseCurrency, string quoteCurrency, Object data, string candleChartId = null);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _serviceProvider;

        private BinanceSocketClient _binanceHubConnection { get; }

        private ExchangeCallback _updateMarketOrderBook { get; }
        private ExchangeCallback _updateCandleChart { get; }

        private readonly IEventBus _eventBus;


        public ExchangeAccessWebsocketService(IServiceProvider serviceProvider, IServiceScopeFactory scopeFactory, IEventBus eventBus)
        {
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

            _updateMarketOrderBook = CreateMarketDataCallback(_serviceProvider);
            _updateCandleChart = CreateCandleCallback(_serviceProvider);

            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            //Binance
            _binanceHubConnection = new BinanceSocketClient();

        }


        public async Task ListenToMarketOrderBook(string marketId)
        {
            //bool retry = true;
            string pair = string.Empty;
            int sizeLimits = 0;
            string baseCurrency = string.Empty;
            string quoteCurrency = string.Empty;
            int? exchangeId = null;
            string exchangeName = string.Empty;

            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var marketRepository = scope.ServiceProvider.GetRequiredService<IMarketRepository>();

                var market = await marketRepository.GetByMarketIdAsync(marketId);
                if (market == null)
                {
                    throw new KeyNotFoundException($"Market with Id \"{marketId}\" not found.");
                }


                pair = market.BaseCurrency + market.QuoteCurrency;
                sizeLimits = market.OrderSizeLimit;

                baseCurrency = market.BaseCurrency;
                quoteCurrency = market.QuoteCurrency;

                exchangeId = market.Exchange.ExchangeId;
                exchangeName = market.Exchange.ExchangeName;

            }

            var success = _binanceHubConnection.SubscribeToPartialBookDepthStream(pair.ToLower(), 10,
                          (data) =>
                          {
                              //_updateMarketOrderBook?.Invoke(exchangeId ?? throw new ArgumentNullException(nameof(exchangeId)), baseCurrency.ToUpper(), quoteCurrency.ToUpper(), data, null);

                              OrderBook bookToPublish = null;
                              if (exchangeId == 0)
                              {
                                  var orderBook = data as BinanceOrderBook;

                                  var askList = new List<Ask>();
                                  var bidList = new List<Bid>();

                                  foreach (var ask in orderBook.Asks)
                                  {
                                      askList.Add(new Ask(
                                          ask.Quantity,
                                          ask.Price
                                          ));
                                  }
                                  foreach (var bid in orderBook.Bids)
                                  {
                                      bidList.Add(new Bid(
                                          bid.Quantity,
                                          bid.Price
                                          ));
                                  }

                                  bookToPublish = new OrderBook(baseCurrency.ToUpper(), quoteCurrency.ToUpper(), askList, bidList);

                              }

                              if (bookToPublish != null)
                              {

                                  var @event = new ExchangeOrderBookDataReceivedIntegrationEvent(
                                      exchangeId ?? throw new ArgumentNullException(nameof(exchangeId)),
                                      bookToPublish
                                      );

                                  this._eventBus.Publish(@event);
                                  //.Publish(new TimeForUpdateBalanceIntegrationEvent());
                              }
                          });


            /*if (retry != true)
            {
                Console.WriteLine($"Subcribe to {exchangeName} order book {pair} success.");
            }
            else
            {
                Console.WriteLine($"Subcribe to {exchangeName} order book {pair} failed.");
            }
            */
        }

        public async Task ListenToMarketCandle(string candleChartId)
        {
            string pair = string.Empty;
            KlineInterval klineInterval;
            string baseCurrency = string.Empty;
            string quoteCurrency = string.Empty;
            int? exchangeId = null;

            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var candleChartRepository = scope.ServiceProvider.GetRequiredService<ICandleChartRepository>();

                var candleChart = await candleChartRepository.GetByCandleChartIdAsync(candleChartId);
                if (candleChart == null)
                {
                    throw new KeyNotFoundException($"Candle chart with Id \"{candleChartId}\" not found.");
                }

                pair = candleChart.BaseCurrency.ToLower() + candleChart.QuoteCurrency.ToLower();
                klineInterval = this.GetKLineIntervalFromCalndlePeriod(candleChart.CandlePeriod);
                baseCurrency = candleChart.BaseCurrency;
                quoteCurrency = candleChart.QuoteCurrency;
                exchangeId = candleChart.ExchangeId;
            }


            /*if (klineInterval != KlineInterval.OneMinute)
            {
                //For testing.
                return;
            }*/

            var success = _binanceHubConnection.SubscribeToKlineStream(pair, klineInterval,
                (data) =>
                {

                    Application.Models.Candle candle = null;

                    if (exchangeId == 0)
                    {
                        var klineData = data as BinanceStreamKlineData;
                        var binanceCandle = klineData.Data;

                        if (binanceCandle.Final != true)
                        {
                            //Only store completed candle.
                            return;
                        }

                        candle = new Application.Models.Candle
                        {
                            Timestamp = binanceCandle.OpenTime,
                            Open = binanceCandle.Open,
                            Close = binanceCandle.Close,
                            High = binanceCandle.High,
                            Low = binanceCandle.Low,
                            Volume = binanceCandle.Volume
                        };
                    }

                    if (candle != null)
                    {
                        var @event = new ExchangeCandleDataReceivedIntegrationEvent(
                            exchangeId ?? throw new ArgumentNullException(nameof(exchangeId)),
                            candleChartId,
                            baseCurrency.ToUpper(),
                            quoteCurrency.ToUpper(),
                            candle
                            );

                        this._eventBus.Publish(@event);
                    }

                });




        }

        public async Task ListenToMarketTicker(string marketId)
        {
            //bool retry = true;
            string pair = string.Empty;
            int sizeLimits = 0;
            string baseCurrency = string.Empty;
            string quoteCurrency = string.Empty;
            int? exchangeId = null;
            string exchangeName = string.Empty;

            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var marketRepository = scope.ServiceProvider.GetRequiredService<IMarketRepository>();

                var market = await marketRepository.GetByMarketIdAsync(marketId);
                if (market == null)
                {
                    throw new KeyNotFoundException($"Market with Id \"{marketId}\" not found.");
                }


                pair = market.BaseCurrency + market.QuoteCurrency;
                sizeLimits = market.OrderSizeLimit;

                baseCurrency = market.BaseCurrency;
                quoteCurrency = market.QuoteCurrency;

                exchangeId = market.Exchange.ExchangeId;
                exchangeName = market.Exchange.ExchangeName;

            }

            var success =_binanceHubConnection.SubscribeToSymbolTicker(pair.ToLower(), (data) =>
            {
                var lastPrice = data.CurrentDayClosePrice;
                if (exchangeId == 0)
                {
                    this._eventBus.Publish(new ExchangeMarketPriceUpdatedIntegrationEvent((int)exchangeId, baseCurrency, quoteCurrency, lastPrice));
                }
            });
            /*var success = _binanceHubConnection.SubscribeToTradesStream(pair.ToLower(), (data) =>
            {
                var lastPrice = data.Price;

               
            });*/


            /*if (retry != true)
            {
                Console.WriteLine($"Subcribe to {exchangeName} order book {pair} success.");
            }
            else
            {
                Console.WriteLine($"Subcribe to {exchangeName} order book {pair} failed.");
            }
            */
        }

        public async Task ListenToAccountBalance(string balanceId)
        {
            int? exchangeId = null;
            var listenKey = string.Empty;

            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var balanceRepository = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();

                var balance = await balanceRepository.GetByBalanceIdAsync(balanceId);
                if (balance == null)
                {
                    throw new KeyNotFoundException($"Balance with Id \"{balanceId}\" not found.");
                }

                exchangeId = balance.ExchangeId;

                using (var client = new BinanceClient())
                {
                    client.SetApiCredentials(balance.Account.ApiKey, balance.Account.ApiSecret);
                    listenKey = client.StartUserStream().Data.ListenKey;
                }
            }

            //Subscribe
            var success = _binanceHubConnection.SubscribeToUserStream(listenKey ?? throw new Exception("Listen key for binance websocket connection not provided."),
                          (accountInfoUpdate) =>
                          {
                              var binanceBalances = accountInfoUpdate.Balances;


                              var assets = new List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Asset>();

                              foreach (var balance in binanceBalances)
                              {
                                  assets.Add(new CryptoTrading.Services.ExchangeAccess.API.Application.Models.Asset
                                  {
                                      Total = balance.Total,
                                      Available = balance.Free,
                                      Locked = balance.Locked,
                                      Symbol = balance.Asset.ToUpper()
                                  });
                              }


                              if (assets.Any())
                              {

                                  var @event = new ExchangeBalanceDataReceivedIntegrationEvent(
                                      exchangeId ?? throw new ArgumentNullException(nameof(exchangeId)),
                                      balanceId,
                                      assets
                                      );

                                  this._eventBus.Publish(@event);
                              }
                          },
                          (orderInfoUpdate) =>
                          {

                          });


            /*if (retry != true)
            {
                Console.WriteLine($"Subcribe to {exchangeName} order book {pair} success.");
            }
            else
            {
                Console.WriteLine($"Subcribe to {exchangeName} order book {pair} failed.");
            }
            */
        }

        public ExchangeCallback CreateMarketDataCallback(IServiceProvider serviceProvider)
        {
            return async (exchangeId, baseCurrency, quoteCurrency, data, candleChartId) =>
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var marketRepository = scope.ServiceProvider.GetRequiredService<IMarketRepository>();

                    var existingMarket = await marketRepository.GetByCurrencyPairAsync(baseCurrency.ToUpper(), quoteCurrency.ToUpper(), exchangeId);

                    if (existingMarket == null)
                    {
                        throw new KeyNotFoundException($"Market with symbol {baseCurrency + quoteCurrency} for exchange {exchangeId} not found");
                    }

                    if (exchangeId == 0)
                    {
                        var orderBook = data as BinanceOrderBook;

                        var askList = new List<Ask>();
                        var bidList = new List<Bid>();

                        foreach (var ask in orderBook.Asks)
                        {
                            askList.Add(new Ask(
                                ask.Quantity,
                                ask.Price
                                ));
                        }
                        foreach (var bid in orderBook.Bids)
                        {
                            bidList.Add(new Bid(
                                bid.Quantity,
                                bid.Price
                                ));
                        }


                        existingMarket.UpdateEntireOrderBook(askList, bidList);


                        marketRepository.Update(existingMarket);




                    }


                    var saved = false;

                    while (!saved)
                    {
                        try
                        {
                            await marketRepository.UnitOfWork.SaveEntitiesAsync();
                            saved = true;
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {

                            foreach (var entry in ex.Entries)
                            {

                                var proposedValues = entry.CurrentValues;
                                var databaseValues = entry.GetDatabaseValues();

                                foreach (var property in proposedValues.Properties)
                                {
                                    var proposedValue = proposedValues[property];
                                    var databaseValue = databaseValues != null ? databaseValues[property] : proposedValue;

                                    // TODO: decide which value should be written to database
                                    proposedValues[property] = proposedValue;
                                }
                            }

                            Console.WriteLine("Exception Solved: DbUpdateConcurrencyException.");
                        }
                    }

                }
            };
        }

        public static ExchangeCallback CreateCandleCallback(IServiceProvider serviceProvider)
        {
            return (exchangeId, baseCurrency, quoteCurrency, data, candleChartId) =>
            {










            };
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
    }
}
