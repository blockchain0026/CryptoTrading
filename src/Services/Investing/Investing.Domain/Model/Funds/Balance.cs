using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Funds
{
    public class Balance :ValueObject
    {
        public Balance(string symbol, decimal total, decimal available, decimal locked)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Total = total;
            Available = available;
            Locked = locked;
        }

        public string Symbol { get; private set; }
        public decimal Total { get; private set; }
        public decimal Available { get; private set; }
        public decimal Locked { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Symbol;
            yield return this.Total;
            yield return this.Available;
            yield return this.Locked;
        }
    }
}
