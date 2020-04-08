using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Funds
{
    public class Account : ValueObject
    {
        public Account(string username)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
        }

        public string Username { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Username;
        }
    }
}
