using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.Domain.Model.Funds
{
    public interface IFundRepository : IRepository<Fund>
    {
        Fund Add(Fund fund);
        Fund Update(Fund fund);
        Task<Fund> GetByFundId(string fundId);
        Task<IEnumerable<Fund>> GetByAccount(Account account);
        Task<IEnumerable<Fund>> GetBySymbol(string symbol);
        Task<IEnumerable<Fund>> GetAll();
    }
}
