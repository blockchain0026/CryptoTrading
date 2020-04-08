using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public interface ITraceRepository : IRepository<Trace>
    {
        Trace Add(Trace trace);
        Trace Update(Trace trace);
        Task<Trace> GetByTraceIdAsync(string traceId);
        Task<Trace> GetByMarketAsync(Market market);
        Task<Trace> GetByInvestmentId(string investmentId);
        Task<IQueryable<Trace>> GetByStatus(TraceStatus traceStatus);
        Task<IQueryable<Trace>> GetAll();
    }
}
