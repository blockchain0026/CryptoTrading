using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.Repositories
{
    public class MarketRepository : IMarketRepository
    {
        private readonly ExchangeAccessContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public MarketRepository(ExchangeAccessContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public Market Add(Market market)
        {
            if(market.IsTransient())
            {
                return _context.Markets
                    .Add(market)
                    .Entity;
            }
            else
            {
                return market;
            }
        }

        public Market Update(Market market)
        {
            return _context.Markets.Update(market).Entity;
            //_context.Entry(market).State = EntityState.Modified;
            //return market;
        }

        public async Task<Market> GetByCurrencyPairAsync(string baseCurrency, string quoteCurrency, int exchangeId)
        {
            var market = await _context.Markets
                .Include(m=>m.Asks)
                .Include(m=>m.Bids)
                .Where(m => m.Exchange.ExchangeId == exchangeId && m.BaseCurrency == baseCurrency && m.QuoteCurrency == quoteCurrency)
                .SingleOrDefaultAsync();

            return market;
        }

        public async Task<Market> GetByMarketIdAsync(string marketId)
        {
            var market = await _context.Markets
                .Include(m => m.Asks)
                .Include(m => m.Bids)
                .Where(m=>m.MarketId == marketId)
                .SingleOrDefaultAsync();

            return market;
        }

        public async Task<IQueryable<Market>> GetAll()
        {
            return _context.Markets
                .Include(m => m.Asks)
                .Include(m => m.Bids);
        }
    }
}
