using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.Repositories
{
    public class TraceRepository : ITraceRepository
    {
        private readonly TrendAnalysisContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public TraceRepository(TrendAnalysisContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Trace Add(Trace trace)
        {
            if (trace.IsTransient())
            {
                return _context.Traces
                    .Add(trace)
                    .Entity;
            }
            else
            {
                return trace;
            }
        }

        public Trace Update(Trace trace)
        {
            return _context.Traces.Update(trace).Entity;

            //_context.Entry(candleChart).State = EntityState.Modified;
            //return candleChart;
        }

        public async Task<Trace> GetByTraceIdAsync(string traceId)
        {
            var trace = await _context.Traces
                .Include(t => t.TradeStrategies)
                .Include(t => t.TradeAdvices)
                .Where(t => t.TraceId == traceId)
                .SingleOrDefaultAsync();

            if (trace != null)
            {
                await _context.Entry(trace)
                      .Reference(t => t.TraceStatus).LoadAsync();
                foreach (var strategy in trace.TradeStrategies)
                {
                    await _context.Entry(strategy.TradeSignal)
                        .Reference(s => s.TradingSignalType).LoadAsync();
                    await _context.Entry(strategy.Strategy)
                        .Reference(s => s.CandlePeriod).LoadAsync();
                }
                foreach (var advice in trace.TradeAdvices)
                {
                    await _context.Entry(advice)
                        .Reference(a => a.TradingSignalType).LoadAsync();
                }
            }

            return trace;
        }

        public async Task<Trace> GetByMarketAsync(Market market)
        {
            var traces = _context.Traces
                 .Include(t => t.TradeStrategies)
                 .Include(t => t.TradeAdvices)
                 .Where(t => t.Market == market);


            if (traces.Any())
            {
                foreach (var trace in traces)
                {
                    await _context.Entry(trace)
                     .Reference(t => t.TraceStatus).LoadAsync();
                    foreach (var strategy in trace.TradeStrategies)
                    {
                        await _context.Entry(strategy.TradeSignal)
                       .Reference(s => s.TradingSignalType).LoadAsync();
                        await _context.Entry(strategy.Strategy)
                            .Reference(s => s.CandlePeriod).LoadAsync();
                    }
                    foreach (var advice in trace.TradeAdvices)
                    {
                        await _context.Entry(advice)
                            .Reference(a => a.TradingSignalType).LoadAsync();
                    }
                }

                //return traces;
            }

            return null;
        }

        public async Task<IQueryable<Trace>> GetAll()
        {
            var traces = _context.Traces
                   .Include(t => t.TradeStrategies)
                   .Include(t => t.TradeAdvices);

            if (traces.Any())
            {
                foreach (var trace in traces)
                {
                    await _context.Entry(trace)
                     .Reference(t => t.TraceStatus).LoadAsync();
                    foreach (var strategy in trace.TradeStrategies)
                    {
                        await _context.Entry(strategy.TradeSignal)
                         .Reference(s => s.TradingSignalType).LoadAsync();
                        await _context.Entry(strategy.Strategy)
                            .Reference(s => s.CandlePeriod).LoadAsync();
                    }
                    foreach (var advice in trace.TradeAdvices)
                    {
                        await _context.Entry(advice)
                            .Reference(a => a.TradingSignalType).LoadAsync();
                    }
                }
            }

            return traces;
        }

        public async Task<Trace> GetByInvestmentId(string investmentId)
        {
            var trace = await _context.Traces
                .Include(t => t.TradeStrategies)
                .Include(t => t.TradeAdvices)
                .Where(t => t.Investment.InvestmentId == investmentId)
                .SingleOrDefaultAsync();

            if (trace != null)
            {
                await _context.Entry(trace)
                      .Reference(t => t.TraceStatus).LoadAsync();
                foreach (var strategy in trace.TradeStrategies)
                {
                    await _context.Entry(strategy.TradeSignal)
                        .Reference(s => s.TradingSignalType).LoadAsync();
                    await _context.Entry(strategy.Strategy)
                        .Reference(s => s.CandlePeriod).LoadAsync();
                }
                foreach (var advice in trace.TradeAdvices)
                {
                    await _context.Entry(advice)
                        .Reference(a => a.TradingSignalType).LoadAsync();
                }
            }

            return trace;
        }

        public async Task<IQueryable<Trace>> GetByStatus(TraceStatus traceStatus)
        {
            var traces = _context.Traces
                .Include(t => t.TradeStrategies)
                .Include(t => t.TradeAdvices);

            var result = new List<Trace>();

            if (traces.Any())
            {
                foreach (var trace in traces)
                {
                    await _context.Entry(trace)
                        .Reference(t => t.TraceStatus).LoadAsync();

                    if (trace.TraceStatus.Id == traceStatus.Id)
                    {
                        foreach (var strategy in trace.TradeStrategies)
                        {
                            await _context.Entry(strategy.TradeSignal)
                                .Reference(s => s.TradingSignalType).LoadAsync();
                            await _context.Entry(strategy.Strategy)
                                .Reference(s => s.CandlePeriod).LoadAsync();
                        }
                        foreach (var advice in trace.TradeAdvices)
                        {
                            await _context.Entry(advice)
                                .Reference(a => a.TradingSignalType).LoadAsync();
                        }
                        result.Add(trace);
                    }
                }

                return result.AsQueryable();
            }




            return traces;
        }
    }
}
