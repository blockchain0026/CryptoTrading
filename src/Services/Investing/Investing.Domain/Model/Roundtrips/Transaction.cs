using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Roundtrips
{
    public class Transaction : ValueObject
    {
        public Transaction(decimal? buyPrice = null, decimal? buyAmount = null, decimal? sellPrice = null, decimal? sellAmount = null)
        {
            BuyPrice = buyPrice;
            BuyAmount = buyAmount;
            SellPrice = sellPrice;
            SellAmount = sellAmount;
        }

        public decimal? BuyPrice { get; private set; }
        public decimal? BuyAmount { get; private set; }
        public decimal? SellPrice { get; private set; }
        public decimal? SellAmount { get; private set; }

        public Transaction BuyOrderFilled(decimal filledAmount, decimal filledPrice)
        {
            return new Transaction(filledPrice, filledAmount);
        }
        public Transaction SellOrderFilled(decimal filledAmount, decimal filledPrice)
        {
            return new Transaction(this.BuyPrice, this.BuyAmount, filledPrice, filledAmount);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.BuyPrice;
            yield return this.BuyAmount;
            yield return this.SellPrice;
            yield return this.SellAmount;
        }
    }
}
