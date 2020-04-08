using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public interface IMarketRepository : IRepository<Market>
    {
        Market Add(Market market);
        Market Update(Market market);
        Task<IQueryable<Market>> GetAll();
        Task<Market> GetByMarketIdAsync(string marketId);
        Task<Market> GetByCurrencyPairAsync(string baseCurrency, string quoteCurrency, int exchangeId);
    }
}
