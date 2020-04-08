using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{

    public static class MarketOrderSortingService
    {
        public static IEnumerable<Ask> SortAskOrders(List<Ask> asks)
        {
            return asks.OrderBy(o => o.Price, new PriceComparaer()).AsEnumerable();
        }

        public static IEnumerable<Bid> SortBidOrders(List<Bid> bids)
        {
            return bids.OrderByDescending(o => o.Price, new PriceComparaer()).AsEnumerable();
        }

        class PriceComparaer : IComparer<decimal>
        {
            public int Compare(decimal x, decimal y)
            {
                return x.CompareTo(y);
            }
        }
    }

 
}
