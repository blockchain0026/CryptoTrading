using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class Exchange : ValueObject
    {
        public int ExchangeId { get; private set; }
        public string ExchangeName { get; private set; }

        public Exchange(int exchangeId, string exchangeName)
        {
            this.ExchangeId = exchangeId;
            this.ExchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.ExchangeId;
            yield return this.ExchangeName;
        }
    }


}