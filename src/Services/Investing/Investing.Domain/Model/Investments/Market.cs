using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Investments
{
    public class Market : ValueObject
    {
        public Market(int exchangeId, string baseCurrency, string quoteCurrency)
        {
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
        }

        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.ExchangeId;
            yield return this.BaseCurrency;
            yield return this.QuoteCurrency;
        }
    }
}
