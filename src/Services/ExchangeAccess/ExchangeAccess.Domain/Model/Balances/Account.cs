using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances
{
    public class Account:ValueObject
    {

        public string Username { get; private set; }
        public string ApiKey { get; private set; }
        public string ApiSecret { get; private set; }

        public Account(string username, string apiKey, string apiSecret)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            ApiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Username;
            yield return this.ApiKey;
            yield return this.ApiSecret;
        }
    }
}
