using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.Infrastructure.Repositories
{
    public class FundRepository : IFundRepository
    {
        private readonly InvestingContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public FundRepository(InvestingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public Fund Add(Fund fund)
        {
            if (fund.IsTransient())
            {
                return _context.Funds
                    .Add(fund)
                    .Entity;
            }
            else
            {
                return fund;
            }
        }

        public Fund Update(Fund fund)
        {
            return _context.Funds.Update(fund).Entity;
        }


        public async Task<Fund> GetByFundId(string fundId)
        {
            var fund = await _context.Funds
                .Include(f => f.InvestingFunds)
                .Where(i => i.FundId == fundId)
                .SingleOrDefaultAsync();

            if (fund != null)
            {
                foreach (var investingFund in fund.InvestingFunds)
                {
                    await _context.Entry(investingFund)
                        .Reference(i => i.InvestingFundStatus).LoadAsync();
                }
            }

            return fund;
        }


        public async Task<IEnumerable<Fund>> GetByAccount(Account account)
        {
            var funds = _context.Funds
                .Include(f => f.InvestingFunds)
                .Where(f => f.Account.Username == account.Username);

            if (funds.Any())
            {
                foreach (var fund in funds)
                {
                    foreach (var investingFund in fund.InvestingFunds)
                    {
                        await _context.Entry(investingFund)
                            .Reference(i => i.InvestingFundStatus).LoadAsync();
                    }
                }

                return funds;
            }

            return new List<Fund>();
        }


        public async Task<IEnumerable<Fund>> GetBySymbol(string symbol)
        {
            var funds = _context.Funds
               .Include(f => f.InvestingFunds)
               .Where(f => f.Symbol == symbol);

            if (funds.Any())
            {
                foreach (var fund in funds)
                {
                    foreach (var investingFund in fund.InvestingFunds)
                    {
                        await _context.Entry(investingFund)
                            .Reference(i => i.InvestingFundStatus).LoadAsync();
                    }
                }

                return funds;
            }

            return new List<Fund>();
        }

        public async Task<IEnumerable<Fund>> GetAll()
        {
            var funds = _context.Funds
                .Include(f => f.InvestingFunds);

            if (funds.Any())
            {
                foreach (var fund in funds)
                {
                    foreach (var investingFund in fund.InvestingFunds)
                    {
                        await _context.Entry(investingFund)
                            .Reference(i => i.InvestingFundStatus).LoadAsync();
                    }
                }

                return funds;
            }

            return new List<Fund>();
        }

    }
}
