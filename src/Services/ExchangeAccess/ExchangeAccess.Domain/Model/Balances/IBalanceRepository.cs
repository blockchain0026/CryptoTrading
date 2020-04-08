using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances
{

    public interface IBalanceRepository : IRepository<Balance>
    {
        Balance Add(Balance balance);
        Balance Update(Balance balance);
        Task<Balance> GetByBalanceIdAsync(string balanceId);
        Task<Balance> GetByUsernameAsync(string username);
        Task<IQueryable<Balance>> GetAll();
    }
}
