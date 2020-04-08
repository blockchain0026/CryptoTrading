using CryptoTrading.Services.ExchangeAccess.Domain.Exceptions;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances
{
    public class Asset : Entity
    {

        public string Symbol { get; private set; }
        public decimal Available { get; private set; }
        public decimal Locked { get; private set; }

        public Asset(string symbol, decimal available, decimal locked)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Available = available >= 0 ? available : throw new ArgumentOutOfRangeException(nameof(available));
            Locked = locked >= 0 ? locked : throw new ArgumentOutOfRangeException(nameof(locked));
        }


        public decimal TotalBalance()
        {
            return this.Available + this.Locked;
        }

        public void AvailableUpdated(decimal availbleBalances)
        {
            if (availbleBalances < 0)
            {
                throw new ExchangeAccessDomainException($"The available balance should be more than zero.");
            }
            this.Available = availbleBalances;
        }

        public void LockedUpdated(decimal lockedBalances)
        {
            if (lockedBalances < 0)
            {
                throw new ExchangeAccessDomainException($"The locked balance should be more than zero.");
            }
            this.Locked = lockedBalances;
        }


    }
}
