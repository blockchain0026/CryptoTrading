using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class Ask:ValueObject
    {
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }

        public Ask(decimal quantity,decimal price)
        {
            this.Quantity = quantity;
            this.Price = price;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Quantity;
            yield return this.Price;
        }
    }
}
