using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class OrderType : Enumeration
    {
        public static OrderType BUY_LIMIT = new OrderType(1, nameof(BUY_LIMIT).ToLowerInvariant());
        public static OrderType SELL_LIMIT = new OrderType(2, nameof(SELL_LIMIT).ToLowerInvariant());

        protected OrderType()
        {
        }

        public OrderType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderType> List() =>
            new[] { BUY_LIMIT, SELL_LIMIT };

        public static OrderType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for OrderType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for OrderType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}