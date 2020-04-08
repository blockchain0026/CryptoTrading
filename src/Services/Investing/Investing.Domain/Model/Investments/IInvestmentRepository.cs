using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.Domain.Model.Investments
{
    public interface IInvestmentRepository : IRepository<Investment>
    {
        Investment Add(Investment investment);
        Investment Update(Investment investment);
        Task<Investment> GetByInvestmentId(string investmentId);
        Task<Investment> GetByTrace(Trace trace);
        Task<IEnumerable<Investment>> GetByMarket(Market market);
        Task<IEnumerable<Investment>> GetByStatus(InvestmentStatus status);
        Task<IEnumerable<Investment>> GetByType(InvestmentType investmentType);
        Task<IEnumerable<Investment>> GetAll();
    }
}
