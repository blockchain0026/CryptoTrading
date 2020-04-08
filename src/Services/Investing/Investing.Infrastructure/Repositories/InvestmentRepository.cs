using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.Infrastructure.Repositories
{
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly InvestingContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public InvestmentRepository(InvestingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public Investment Add(Investment investment)
        {
            if (investment.IsTransient())
            {
                return _context.Investments
                    .Add(investment)
                    .Entity;
            }
            else
            {
                return investment;
            }
        }
        public Investment Update(Investment investment)
        {
            return _context.Investments.Update(investment).Entity;
        }

        public async Task<Investment> GetByInvestmentId(string investmentId)
        {
            var investment = await _context.Investments
               .Where(i => i.InvestmentId == investmentId)
               .Include(i => i.InvestmentRoundtrips)
               .SingleOrDefaultAsync();

            if (investment != null)
            {
                await _context.Entry(investment)
                      .Reference(i => i.InvestmentStatus).LoadAsync();
                await _context.Entry(investment)
                    .Reference(i => i.InvestmentType).LoadAsync();
            }

            return investment;
        }


        public async Task<Investment> GetByTrace(Trace trace)
        {
            var investment = await _context.Investments
               .Include(i => i.InvestmentRoundtrips)
               .Where(i => i.Trace.TraceId == trace.TraceId)
               .SingleOrDefaultAsync();

            if (investment != null)
            {
                await _context.Entry(investment)
                      .Reference(i => i.InvestmentStatus).LoadAsync();
                await _context.Entry(investment)
                    .Reference(i => i.InvestmentType).LoadAsync();
            }

            return investment;
        }



        public async Task<IEnumerable<Investment>> GetByMarket(Market market)
        {
            var investments = _context.Investments
                .Include(i => i.InvestmentRoundtrips)
                .Where(i => i.Market.BaseCurrency == market.BaseCurrency
                && i.Market.QuoteCurrency == market.QuoteCurrency
                && i.Market.ExchangeId == market.ExchangeId);

            if (investments.Any())
            {
                foreach (var investment in investments)
                {
                    await _context.Entry(investment)
                     .Reference(i => i.InvestmentStatus).LoadAsync();
                    await _context.Entry(investment)
                        .Reference(i => i.InvestmentType).LoadAsync();
                }

                return investments;
            }

            return new List<Investment>();
        }

        public async Task<IEnumerable<Investment>> GetByStatus(InvestmentStatus status)
        {
            var investments = _context.Investments
              .Include(i => i.InvestmentRoundtrips);

            var result = new List<Investment>();

            foreach (var investment in investments)
            {
                await _context.Entry(investment)
                     .Reference(i => i.InvestmentStatus).LoadAsync();
                if (investment.InvestmentStatus.Id == status.Id)
                {
                    await _context.Entry(investment)
                        .Reference(i => i.InvestmentType).LoadAsync();

                    result.Add(investment);
                }
            }

            return result;
        }


        public async Task<IEnumerable<Investment>> GetByType(InvestmentType investmentType)
        {
            var investments = _context.Investments
             .Include(i => i.InvestmentRoundtrips);

            var result = new List<Investment>();

            foreach (var investment in investments)
            {
                await _context.Entry(investment)
                     .Reference(i => i.InvestmentType).LoadAsync();
                if (investment.InvestmentType.Id == investmentType.Id)
                {
                    await _context.Entry(investment)
                        .Reference(i => i.InvestmentStatus).LoadAsync();

                    result.Add(investment);
                }
            }

            return result;
        }


        public async Task<IEnumerable<Investment>> GetAll()
        {
            var investments = _context.Investments
                .Include(i => i.InvestmentRoundtrips);

            if (investments.Any())
            {
                foreach (var investment in investments)
                {
                    await _context.Entry(investment)
                     .Reference(i => i.InvestmentStatus).LoadAsync();
                    await _context.Entry(investment)
                        .Reference(i => i.InvestmentType).LoadAsync();
                }

                return investments;
            }

            return new List<Investment>();
        }







    }
}
