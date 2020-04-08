using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts
{
    public interface ICandleChartRepository : IRepository<CandleChart>
    {
        CandleChart Add(CandleChart candleChart);
        CandleChart Update(CandleChart candleChart);
        Task<CandleChart> GetByCandleChartIdAsync(string candleChartId);
        Task<CandleChart> GetByCurrencyPairAsync(string baseCurrency, string quoteCurrency, int exchangeId, CandlePeriod candlePeriod);
        Task<IQueryable<CandleChart>> GetAll();
    }
}
