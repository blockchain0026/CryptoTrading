using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.Domain.Model.Roundtrips
{
    public interface IRoundtripRepository : IRepository<Roundtrip>
    {
        Roundtrip Add(Roundtrip roundtrip);
        Roundtrip Update(Roundtrip roundtrip);
        Task<Roundtrip> GetByRoundtripId(string roundtripId);
        Task<IEnumerable<Roundtrip>> GetByInvestmentId(string investmentId);
        Task<IEnumerable<Roundtrip>> GetByStatus(RoundtripStatus status);
        Task<IEnumerable<Roundtrip>> GetAll();
    }
}
