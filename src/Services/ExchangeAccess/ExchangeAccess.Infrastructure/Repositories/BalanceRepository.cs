using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.Repositories
{
    public class BalanceRepository : IBalanceRepository
    {
        private readonly ExchangeAccessContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public BalanceRepository(ExchangeAccessContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Balance Add(Balance balance)
        {
            if (balance.IsTransient())
            {
                return _context.Balances
                    .Add(balance)
                    .Entity;
            }
            else
            {
                return balance;
            }
        }

        public Balance Update(Balance balance)
        {
            return _context.Balances.Update(balance).Entity;
        }

        public async Task<Balance> GetByBalanceIdAsync(string balanceId)
        {
            var balance = await _context.Balances.Include(b => b.Assets).Where(b => b.BalanceId == balanceId).SingleOrDefaultAsync();
            return balance;
        }

        public async Task<Balance> GetByUsernameAsync(string username)
        {
            var balance = await _context.Balances.Include(b => b.Assets).Where(b => b.Account.Username == username).SingleOrDefaultAsync();
            return balance;
        }

        public async Task<IQueryable<Balance>> GetAll()
        {
            var balances = _context.Balances
                .Include(b => b.Assets);

            return balances;
        }
    }
}
