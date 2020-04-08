using Binance.Net;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using CryptoTrading.Services.ExchangeAccess.Infrastructure;
using CryptoTrading.Services.ExchangeAccess.Infrastructure.DomainServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Services
{
    public class ExchangeAccessHostService : IHostedService
    {
        private const string DEFAULT_APIKEY = "FPjCm7qHt0gRKhTRpQqzEXJftTzrnnyMfd7gNDrCVMykLV8sD7hX1O8Ds82tMCX7";
        private const string DEFAULT_APISECRET = "JaMu3Z25Obri0aSMP1HqZWCH40Gc3Y4SXa5QG52DycDUiQ36eQfFjuJL55HyHAMx";
        private bool _marketConnected = false;
        private bool _chartConnected = false;
        private bool _balanceConnected = false;
        private bool _priceConnected = false;
        private CancellationTokenSource _cts;
        private Task _executingTask;

        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IExchangeAccessWebsocketService _exchangeAccessWebsocketService;



        public ExchangeAccessHostService(IServiceProvider serviceProvider, IServiceScopeFactory scopeFactory, IExchangeAccessWebsocketService exchangeAccessWebsocketService)
        {
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this._exchangeAccessWebsocketService = exchangeAccessWebsocketService ?? throw new ArgumentNullException(nameof(exchangeAccessWebsocketService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            /*var markets = _marketRepository.GetAll().Result;

            foreach (var market in markets)
            {
                this._exchangeAccessService.ListenToMarketOrderBook(market.MarketId);
            }*/

            _executingTask = Task.Run(async () =>
            {
                await this.InitializeMarkets(0);
                await this.InitializeCandles(0);
                await this.InitializeBalance(0);

                //Keep detecting until shutdown.
                while (!_cts.IsCancellationRequested)
                {
                    if (!_marketConnected)
                    {
                        await this.KeepUpdatingMarketDataFromExchange(0);
                    }
                    if (!_chartConnected)
                    {
                        await this.KeepUpdatingCandleChartFromExchange(0);
                    }
                    if (!_balanceConnected)
                    {
                        await this.KeepUpdatingBalanceFromExchange(0);
                    }
                    if (!_priceConnected)
                    {
                        await this.KeepUpdatingMarketPriceFromExchange(0);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            });

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }


        public async Task InitializeMarkets(int exchangeId)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var marketRepo = scope.ServiceProvider.GetRequiredService<IMarketRepository>();
                var exchangeAccessService = scope.ServiceProvider.GetRequiredService<IExchangeAccessService>();

                var marketSymbols = this.GetMarketsNeeded();

                foreach (var symbol in marketSymbols)
                {
                    var market = await marketRepo.GetByCurrencyPairAsync(symbol.Value, symbol.Key, exchangeId);
                    if (market == null)
                    {
                        var toAdd = await Market.FromAccessService(exchangeAccessService, exchangeId, symbol.Value, symbol.Key);
                        if (toAdd != null)
                        {
                            marketRepo.Add(toAdd);
                            await marketRepo.UnitOfWork.SaveEntitiesAsync();
                        }
                    }
                }
            }

        }

        public async Task InitializeCandles(int exchangeId)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var marketRepo = scope.ServiceProvider.GetRequiredService<IMarketRepository>();
                var candleChartRepo = scope.ServiceProvider.GetRequiredService<ICandleChartRepository>();
                var exchangeAccessService = scope.ServiceProvider.GetRequiredService<IExchangeAccessService>();

                var marketSymbols = this.GetMarketsNeeded();

                foreach (var symbol in marketSymbols)
                {
                    var market = await marketRepo.GetByCurrencyPairAsync(symbol.Value, symbol.Key, exchangeId);
                    if (market != null)
                    {
                        foreach (var period in CandlePeriod.List())
                        {
                            var existingCandleChart = await candleChartRepo.GetByCurrencyPairAsync(symbol.Value, symbol.Key, exchangeId, period);
                            if (existingCandleChart == null)
                            {
                                var toAdd = CandleChart.FromMarket(market, period);

                                candleChartRepo.Add(toAdd);
                                await candleChartRepo.UnitOfWork.SaveEntitiesAsync();
                            }
                        }
                    }
                }
            }
        }

        public async Task InitializeBalance(int exchangeId)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var balanceRepo = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();

                var accounts = this.GetAllAccounts();

                foreach (var account in accounts)
                {
                    var existingBalance = await balanceRepo.GetByUsernameAsync(account.Username);

                    if (existingBalance == null)
                    {
                        var toAdd = new Balance(account, exchangeId);
                        if (toAdd != null)
                        {
                            balanceRepo.Add(toAdd);
                            await balanceRepo.UnitOfWork.SaveEntitiesAsync();
                        }
                    }
                }

                //Get current balances
                foreach (var account in accounts)
                {
                    var exchangeAccessService = scope.ServiceProvider.GetRequiredService<IExchangeAccessService>();
                    var eventService = scope.ServiceProvider.GetRequiredService<IExchangeAccessIntegrationEventService>();

                    var balances = await exchangeAccessService.GetAllBalance(account, exchangeId);

                    if (balances.Any())
                    {
                        var existingBalance = await balanceRepo.GetByUsernameAsync(account.Username);
                        if (existingBalance != null)
                        {
                            var assets = new List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Asset>();
                            foreach (var balance in balances)
                            {
                                if (balance.TotalBalance() <= 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    assets.Add(new Application.Models.Asset
                                    {
                                        Symbol = balance.Symbol.ToUpper(),
                                        Available = balance.Available,
                                        Locked = balance.Locked,
                                        Total = balance.TotalBalance()
                                    });
                                }
                            }

                            await eventService.PublishThroughEventBusAsync(new ExchangeBalanceDataReceivedIntegrationEvent(
                                exchangeId,
                                existingBalance.BalanceId,
                                assets
                                ));

                            await eventService.PublishThroughEventBusAsync(new BalanceInitializeIntegrationEvent(
                                exchangeId,
                                existingBalance.BalanceId,
                                account.Username,
                                assets
                                ));
                        }
                    }
                }

            }


        }


        public async Task KeepUpdatingMarketDataFromExchange(int exchangeId)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var marketRepo = scope.ServiceProvider.GetRequiredService<IMarketRepository>();


                var markets = await marketRepo.GetAll();

                foreach (var market in markets)
                {
                    await _exchangeAccessWebsocketService.ListenToMarketOrderBook(market.MarketId);
                }
            }
            this._marketConnected = true;
        }

        public async Task KeepUpdatingCandleChartFromExchange(int exchangeId)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var candleChartRepo = scope.ServiceProvider.GetRequiredService<ICandleChartRepository>();


                var charts = await candleChartRepo.GetAll();


                foreach (var candleChart in charts)
                {
                    await _exchangeAccessWebsocketService.ListenToMarketCandle(candleChart.CandleChartId);
                }
                /**/
            }
            this._chartConnected = true;
        }

        public async Task KeepUpdatingMarketPriceFromExchange(int exchangeId)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var marketRepo = scope.ServiceProvider.GetRequiredService<IMarketRepository>();


                var markets = await marketRepo.GetAll();


                foreach (var market in markets)
                {
                    await _exchangeAccessWebsocketService.ListenToMarketTicker(market.MarketId);
                }
                /**/
            }
            this._priceConnected = true;
        }


        public async Task KeepUpdatingBalanceFromExchange(int exchangeId)
        {
            using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var balanceRepo = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();


                var balances = await balanceRepo.GetAll();

                foreach (var balance in balances)
                {

                    //Start user stream to get listen key.
                    var listenKey = string.Empty;


                    await _exchangeAccessWebsocketService.ListenToAccountBalance(balance.BalanceId);
                }
            }
            this._balanceConnected = true;
        }




        private List<KeyValuePair<string, string>> GetMarketsNeeded()
        {
            var marketSymbols = new List<KeyValuePair<string, string>>();

            //Key = quote currency, Value= base currency.
            marketSymbols.Add(new KeyValuePair<string, string>("USDT", "BTC"));
            marketSymbols.Add(new KeyValuePair<string, string>("USDT", "ETH"));
            return marketSymbols;
        }

        private List<Account> GetAllAccounts()
        {
            var accounts = new List<Account>();

            accounts.Add(new Account("test", DEFAULT_APIKEY, DEFAULT_APISECRET));

            return accounts;
        }

        private Account GetAccountInfo(string username)
        {
            if (username == "test")
            {
                return new Account(username, DEFAULT_APIKEY, DEFAULT_APISECRET);
            }

            return null;
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // 发送停止信号，以通知我们的后台服务结束执行。
            _cts.Cancel();

            // 等待后台服务的停止，而 ASP.NET Core 大约会等待5秒钟（可在上面介绍的UseShutdownTimeout方法中配置），如果还没有执行完会发送取消信号，以防止无限的等待下去。
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

    }
}
