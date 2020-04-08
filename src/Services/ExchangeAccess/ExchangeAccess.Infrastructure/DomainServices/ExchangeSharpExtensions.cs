using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.DomainServices
{
    public static class ExchangeSharpExtensions
    {
        public static OrderBook ToOrderBook(this ExchangeOrderBook source, string baseCurrency, string quoteCurrency)
        {
            var asksList = new List<Ask>();
            var bidsList = new List<Bid>();


            foreach (var order in source.Asks)
            {
                asksList.Add(
                    new Ask(
                        order.Value.Amount,
                        order.Value.Price));
            }

            foreach (var order in source.Bids)
            {
                bidsList.Add(
                    new Bid(
                        order.Value.Amount,
                        order.Value.Price));
            }

            return new OrderBook(baseCurrency, quoteCurrency, asksList, bidsList);
        }

        public static IEnumerable<Order> ToOpenOrders(this IEnumerable<ExchangeOrderResult> source, int exchangeId)
        {
            var orderList = new List<Order>();

            foreach (var order in source)
            {
                var toAdd = new Order(
                    exchangeId,
                    order.OrderId,
                    order.IsBuy ? Domain.Model.Markets.OrderType.BUY_LIMIT : Domain.Model.Markets.OrderType.SELL_LIMIT,
                    order.AmountFilled > 0 ? OrderStatus.PartiallyFilled : OrderStatus.New,
                    order.Symbol.Substring(0, 3),
                    order.Symbol.Substring(4),
                    order.Price,
                    order.Amount,
                    order.AmountFilled,
                    order.Fees,
                    order.OrderDate
                    );

                orderList.Add(toAdd);
            }

            return orderList;
        }


     
    }
}
