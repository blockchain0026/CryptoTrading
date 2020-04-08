using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class OrderBook:ValueObject
    {

        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public IEnumerable<Ask> Asks { get; private set; }
        public IEnumerable<Bid> Bids { get; private set; }


        public OrderBook(string baseCurrency, string quoteCurrency, IEnumerable<Ask> asks, IEnumerable<Bid> bids)
        {
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            Asks = asks ?? throw new ArgumentNullException(nameof(asks));
            Bids = bids ?? throw new ArgumentNullException(nameof(bids));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.BaseCurrency;
            yield return this.QuoteCurrency;
            yield return this.Asks;
            yield return this.Bids;
        }
    }
}
