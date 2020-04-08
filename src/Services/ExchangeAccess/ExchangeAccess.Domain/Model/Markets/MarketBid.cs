using CryptoTrading.Services.ExchangeAccess.Domain.Exceptions;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class MarketBid : Entity
    {
        public string MarketId { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }

        public MarketBid(string marketId,decimal quantity, decimal price)
        {
            this.MarketId = marketId??throw new ArgumentNullException(nameof(marketId));
            this.Quantity = quantity;
            this.Price = price;
        }

        public void UpdateData(decimal quantity, decimal price)
        {
            this.Quantity = quantity;
            this.Price = price;
        }

        public void Clear()
        {
            this.Quantity = 0;
            this.Price = 0;
        }

        public bool IsEmpty()
        {
            return this.Quantity == 0 && this.Price == 0;
        }

        public Bid GetInfo()
        {
            return new Bid(this.Quantity, this.Price);
        }

        public void UpdateQuantity(decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ExchangeAccessDomainException("The quantity must more than zero.");
            }
            this.Quantity = quantity;
        }
    }
}
