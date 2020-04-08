using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class OrderStatus
          : Enumeration
    {
        public static OrderStatus New = new OrderStatus(1, nameof(New).ToLowerInvariant());
        public static OrderStatus PartiallyFilled = new OrderStatus(2, nameof(PartiallyFilled).ToLowerInvariant());
        public static OrderStatus Filled = new OrderStatus(3, nameof(Filled).ToLowerInvariant());
        public static OrderStatus Canceled = new OrderStatus(4, nameof(Canceled).ToLowerInvariant());
        public static OrderStatus Rejected = new OrderStatus(5, nameof(Rejected).ToLowerInvariant());
        public static OrderStatus Expired = new OrderStatus(6, nameof(Expired).ToLowerInvariant());

        protected OrderStatus()
        {
        }

        public OrderStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderStatus> List() =>
            new[] { New, PartiallyFilled, Filled, Canceled, Rejected, Expired};

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}