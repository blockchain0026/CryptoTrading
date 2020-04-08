using CryptoTrading.Services.ExchangeAccess.Domain.Exceptions;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances
{
    public class Balance : Entity, IAggregateRoot
    {
        public string BalanceId { get; private set; }
        public int ExchangeId { get; private set; }
        public Account Account { get; private set; }


        private readonly List<Asset> _assets;
        public IReadOnlyCollection<Asset> Assets => _assets;

        protected Balance()
        {
            this._assets = new List<Asset>();
        }
        public Balance(Account account, int exchangeId) : this()
        {
            this.BalanceId = Guid.NewGuid().ToString();
            this.ExchangeId = exchangeId;
            this.Account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public void AddAsset(string symbol, decimal available, decimal locked)
        {
            var existingAsset = this._assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset != null)
            {
                throw new ExchangeAccessDomainException($"Asset with symbol {symbol} is already exsit.");
            }

            this._assets.Add(
                new Asset(symbol, available, locked));
        }

        public void AssetUpdated(string symbol, decimal available, decimal locked)
        {
            var existingAsset = this._assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset == null)
            {
                throw new ExchangeAccessDomainException($"Asset with symbol {symbol} is not found.");
            }

            existingAsset.AvailableUpdated(available);
            existingAsset.LockedUpdated(locked);
        }

        public decimal GetAvailableBalance(string symbol)
        {
            var existingAsset = this._assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset == null)
            {
                throw new ExchangeAccessDomainException($"Asset with symbol {symbol} is not found.");
            }
            return existingAsset.Available;
        }

        public decimal GetLockedBalance(string symbol)
        {
            var existingAsset = this._assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset == null)
            {
                throw new ExchangeAccessDomainException($"Asset with symbol {symbol} is not found.");
            }
            return existingAsset.Locked;
        }

        public decimal GetTotalBalance(string symbol)
        {
            var existingAsset = this._assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset == null)
            {
                throw new ExchangeAccessDomainException($"Asset with symbol {symbol} is not found.");
            }
            return existingAsset.TotalBalance();
        }
    }
}
