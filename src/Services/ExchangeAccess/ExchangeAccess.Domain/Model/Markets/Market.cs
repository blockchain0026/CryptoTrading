using CryptoTrading.Services.ExchangeAccess.Domain.Exceptions;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class Market : Entity, IAggregateRoot
    {
        #region Properties & Fields

        public string MarketId { get; private set; }

        public Exchange Exchange { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }

        public int OrderSizeLimit { get; private set; }

        private readonly List<MarketAsk> _asks;
        public IReadOnlyCollection<MarketAsk> Asks => _asks;

        private readonly List<MarketBid> _bids;
        public IReadOnlyCollection<MarketBid> Bids => _bids;

        #endregion



        #region Constructor
        protected Market()
        {
            this._asks = new List<MarketAsk>();
            this._bids = new List<MarketBid>();
        }

        public Market(string marketId, Exchange exchange, string baseCurrency, string quoteCurrency) : this()
        {
            this.BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            this.QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            this.MarketId = marketId ?? throw new ArgumentNullException(nameof(marketId));
            this.Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));

            this.OrderSizeLimit = 20;
        }

        #endregion


        #region Functions

        public static async Task<Market> FromAccessService(IExchangeAccessService exchangeAccessService, int exchangeId, string baseCurrency, string quoteCurrency)
        {

            //Ensure the exchange does exist.
            var availbleExchanges = exchangeAccessService.GetAvailableExchanges();
            var exchange = availbleExchanges.Where(e => e.ExchangeId == exchangeId).SingleOrDefault();
            if (exchange == null)
            {
                throw new ExchangeAccessDomainException($"Exchange not found. Exchange Id: {exchangeId}. \n" +
                    $"Available exchanges can be found with function GetAvailableExchanges.");
            }


            //Ensure the market info is right.
            OrderBook orderBook = null;
            try
            {
                orderBook = await exchangeAccessService.GetMarketData(exchange.ExchangeId, baseCurrency, quoteCurrency);
            }
            catch (Exception ex)
            {
                throw new ExchangeAccessDomainException(
                    $"There are errors when trying to get market data on exchange {exchange.ExchangeName} with currency pair {baseCurrency + quoteCurrency}.",
                    ex);
            }


            return new Market(
                Guid.NewGuid().ToString(),
                exchange,
                baseCurrency,
                quoteCurrency
                );
        }


        public void UpdateOrderBook(IEnumerable<Ask> asks, IEnumerable<Bid> bids)
        {
            foreach (var order in asks)
            {
                //To remove.
                if (order.Quantity <= 0)
                {
                    var existingOrder = this._asks.Where(a => a.Price == order.Price).SingleOrDefault();
                    if (existingOrder != null)
                    {
                        this._asks.Remove(existingOrder);
                    }
                }
                else
                {
                    var existingOrder = this._asks.Where(a => a.Price == order.Price).SingleOrDefault();

                    //To update
                    if (existingOrder != null)
                    {
                        existingOrder.UpdateQuantity(order.Quantity);
                    }
                    else // To add.
                    {
                        this._asks.Add(new MarketAsk(
                            this.MarketId,
                            order.Quantity,
                            order.Price
                            ));
                    }

                }
            }


            foreach (var order in bids)
            {
                //To remove.
                if (order.Quantity <= 0)
                {
                    var existingOrder = this._bids.Where(a => a.Price == order.Price).SingleOrDefault();
                    if (existingOrder != null)
                    {
                        this._bids.Remove(existingOrder);
                    }
                }
                else
                {
                    var existingOrder = this._bids.Where(a => a.Price == order.Price).SingleOrDefault();

                    //To update
                    if (existingOrder != null)
                    {
                        existingOrder.UpdateQuantity(order.Quantity);
                    }
                    else // To add.
                    {
                        this._bids.Add(new MarketBid(
                            this.MarketId,
                            order.Quantity,
                            order.Price
                            ));
                    }

                }
            }
        }

        public void UpdateEntireOrderBook(IEnumerable<Ask> asks, IEnumerable<Bid> bids)
        {
            List<Ask> askList = null;
            List<Bid> bidList = null;

            if (asks.Count() > this.OrderSizeLimit)
            {
                askList = MarketOrderSortingService.SortAskOrders(asks.ToList()).Take(OrderSizeLimit).ToList();
            }
            else
            {
                askList = asks.ToList();
            }

            if (bids.Count() > this.OrderSizeLimit)
            {
                bidList = MarketOrderSortingService.SortBidOrders(bids.ToList()).Take(OrderSizeLimit).ToList();
            }
            else
            {
                bidList = bids.ToList();
            }


            foreach (var ask in this._asks)
            {
                ask.Clear();
            }
            foreach (var bid in this._bids)
            {
                bid.Clear();
            }

            if (this._asks.Count < this.OrderSizeLimit)
            {
                var amountToAdd = this.OrderSizeLimit - this._asks.Count;

                for (int i = 0; i < amountToAdd; i++)
                {
                    this._asks.Add(new MarketAsk(this.MarketId, 0, 0));
                }
            }

            if (this._bids.Count < this.OrderSizeLimit)
            {
                var amountToAdd = this.OrderSizeLimit - this._bids.Count;

                for (int i = 0; i < amountToAdd; i++)
                {
                    this._bids.Add(new MarketBid(this.MarketId, 0, 0));
                }
            }



            for (int i = 0; i < askList.Count; i++)
            {
                var ask = askList[i];
                this._asks[i].UpdateData(ask.Quantity, ask.Price);
            }

            for (int i = 0; i < bidList.Count; i++)
            {
                var bid = bidList[i];
                this._bids[i].UpdateData(bid.Quantity, bid.Price);
            }

            /*this._asks.Clear();
            this._bids.Clear();*/

            /*foreach (var ask in askList)
            {
                var existingAsk = this._asks.Where(a => a.Price == ask.Price).SingleOrDefault();
                if (existingAsk != null)
                {
                    existingAsk.UpdateQuantity(ask.Quantity);
                }
                else
                {
                    this._asks.Add(new MarketAsk(
                        this.MarketId,
                        ask.Quantity,
                        ask.Price
                        ));
                }

            }
            foreach (var bid in bidList)
            {
                var existingBid = this._bids.Where(b => b.Price == bid.Price).SingleOrDefault();
                if (existingBid != null)
                {
                    existingBid.UpdateQuantity(bid.Quantity);
                }
                else
                {
                    this._bids.Add(new MarketBid(
                        this.MarketId,
                        bid.Quantity,
                        bid.Price
                        ));
                }

            }*/
        }


        public string CreateBuyOrder(Balance balance, string baseCurrency, string quoteCurrency, decimal price, decimal quantity, IExchangeAccessService exchangeAccessService)
        {
            if (balance.ExchangeId != this.Exchange.ExchangeId)
            {
                throw new ExchangeAccessDomainException("The balance provided doesn't match the exchange this market on.");
            }
            if (balance.GetAvailableBalance(quoteCurrency) < quantity)
            {
                throw new ExchangeAccessDomainException("The account's balances isn't efficient to create buy order.");
            }

            var orderId = exchangeAccessService.CreateBuyOrder(balance.Account, balance.ExchangeId, baseCurrency, quoteCurrency, price, quantity).Result;

            if (orderId == null)
            {
                throw new ExchangeAccessDomainException("There are errors when attempting to create buy order.");
            }

            return orderId;
        }


        public string CreateSellOrder(Balance balance, string baseCurrency, string quoteCurrency, decimal price, decimal quantity, IExchangeAccessService exchangeAccessService)
        {
            if (balance.ExchangeId != this.Exchange.ExchangeId)
            {
                throw new ExchangeAccessDomainException("The balance provided doesn't match the exchange this market on.");
            }

            if (balance.GetAvailableBalance(baseCurrency) < quantity)
            {
                throw new ExchangeAccessDomainException("The account's balances isn't efficient to create sell order.");
            }

            var orderId = exchangeAccessService.CreateSellOrder(balance.Account, balance.ExchangeId, baseCurrency, quoteCurrency, price, quantity).Result;

            if (orderId == null)
            {
                throw new ExchangeAccessDomainException("There are errors when attempting to create sell order.");
            }

            return orderId;
        }


        public void CancelOrder(Account account, string orderId, IExchangeAccessService exchangeAccessService)
        {
            if (!exchangeAccessService.CancelOrder(account, this.Exchange.ExchangeId, orderId, this.BaseCurrency, this.QuoteCurrency).Result)
            {
                throw new ExchangeAccessDomainException($"There are errors when attempting to cancel order. Order Id: {orderId}, Exchange: {this.Exchange.ExchangeName}.");
            }
        }

        public OrderBook GetOrderBook()
        {
            var askList = new List<Ask>();
            var bidList = new List<Bid>(); ;

            foreach (var ask in this._asks)
            {
                if (!ask.IsEmpty())
                {
                    askList.Add(ask.GetInfo());

                }
            }
            foreach (var bid in this._bids)
            {
                if (!bid.IsEmpty())
                {
                    bidList.Add(bid.GetInfo());
                }
            }

            return new OrderBook(
                this.BaseCurrency,
                this.QuoteCurrency,
                askList,
                bidList
                );
        }

        public IEnumerable<Order> GetOpenOrdeer(Account account, IExchangeAccessService exchangeAccessService)
        {
            return exchangeAccessService.GetOpenOrders(account, this.Exchange.ExchangeId).Result;
        }
        #endregion


    }


}
