using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.Repositories
{
    public class CandleChartRepository : ICandleChartRepository
    {
        private readonly ExchangeAccessContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CandleChartRepository(ExchangeAccessContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public CandleChart Add(CandleChart candleChart)
        {
            if (candleChart.IsTransient())
            {
                return _context.CandleCharts
                    .Add(candleChart)
                    .Entity;
            }
            else
            {
                return candleChart;
            }
        }

        public CandleChart Update(CandleChart candleChart)
        {
            return _context.CandleCharts.Update(candleChart).Entity;

            //_context.Entry(candleChart).State = EntityState.Modified;
            //return candleChart;
        }

        public async Task<CandleChart> GetByCandleChartIdAsync(string candleChartId)
        {
            var candleChart = await _context.CandleCharts
                .Include(c => c.Candles)
                .Where(c => c.CandleChartId == candleChartId)
                .SingleOrDefaultAsync();

            if(candleChart!=null)
            {
                await _context.Entry(candleChart)
                      .Reference(c => c.CandlePeriod).LoadAsync();
            }

            return candleChart;
        }

        public async Task<CandleChart> GetByCurrencyPairAsync(string baseCurrency, string quoteCurrency, int exchangeId, CandlePeriod candlePeriod)
        {
            var candleCharts = _context.CandleCharts
                .Include(c => c.Candles)
                .Where(c => c.ExchangeId == exchangeId && c.BaseCurrency == baseCurrency && c.QuoteCurrency == quoteCurrency);

            if (candleCharts.Any())
            {
                foreach (var chart in candleCharts)
                {
                    await _context.Entry(chart)
                        .Reference(c => c.CandlePeriod).LoadAsync();
                    if (chart.CandlePeriod.Id == candlePeriod.Id)
                    {
                        return chart;
                    }
                }

            }

            return null;
        }

        public async Task<IQueryable<CandleChart>> GetAll()
        {
            var charts=_context.CandleCharts
                 .Include(c => c.Candles);

            if (charts.Any())
            {
                foreach (var chart in charts)
                {
                    await _context.Entry(chart)
                        .Reference(c => c.CandlePeriod).LoadAsync();
                }
            }

            return charts;
        }
    }
}
