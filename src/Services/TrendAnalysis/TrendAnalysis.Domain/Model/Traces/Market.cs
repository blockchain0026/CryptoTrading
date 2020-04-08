using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public class Market : ValueObject
    {
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }

        public Market(int exchangeId, string baseCurrency, string quoteCurrency)
        {
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.ExchangeId;
            yield return this.BaseCurrency;
            yield return this.QuoteCurrency;
        }
    }
}
