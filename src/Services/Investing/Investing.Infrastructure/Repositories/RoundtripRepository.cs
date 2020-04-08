using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.Infrastructure.Repositories
{
    public class RoundtripRepository : IRoundtripRepository
    {
        private readonly InvestingContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public RoundtripRepository(InvestingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public Roundtrip Add(Roundtrip roundtrip)
        {
            if (roundtrip.IsTransient())
            {
                return _context.Roundtrips
                    .Add(roundtrip)
                    .Entity;
            }
            else
            {
                return roundtrip;
            }
        }


        public Roundtrip Update(Roundtrip roundtrip)
        {
            return _context.Roundtrips.Update(roundtrip).Entity;
        }


        public async Task<Roundtrip> GetByRoundtripId(string roundtripId)
        {
            var roundtrip = await _context.Roundtrips
              .Where(i => i.RoundtripId == roundtripId)
              .SingleOrDefaultAsync();

            if (roundtrip != null)
            {
                await _context.Entry(roundtrip)
                      .Reference(r => r.RoundtripStatus).LoadAsync();
            }

            return roundtrip;
        }


        public async Task<IEnumerable<Roundtrip>> GetByInvestmentId(string investmentId)
        {
            var roundtrips = _context.Roundtrips
                .Where(i => i.InvestmentId == investmentId);

            if (roundtrips.Any())
            {
                foreach (var roundtrip in roundtrips)
                {
                    await _context.Entry(roundtrip)
                    .Reference(r => r.RoundtripStatus).LoadAsync();
                }

                return roundtrips;
            }

            return new List<Roundtrip>();
        }


        public async Task<IEnumerable<Roundtrip>> GetByStatus(RoundtripStatus status)
        {
            var roundtrips = _context.Roundtrips
                .Where(r => r.GetStatus().Id == status.Id);



            if (roundtrips.Any())
            {
                
                foreach (var roundtrip in roundtrips)
                {
                    await _context.Entry(roundtrip)
                    .Reference(r => r.RoundtripStatus).LoadAsync();

                    
                }

                return roundtrips;
            }

            return new List<Roundtrip>();
        }

        public async Task<IEnumerable<Roundtrip>> GetAll()
        {
            var roundtrips = _context.Roundtrips;

            if (roundtrips.Any())
            {
                foreach (var roundtrip in roundtrips)
                {
                    await _context.Entry(roundtrip)
                    .Reference(r => r.RoundtripStatus).LoadAsync();
                }

                return roundtrips;
            }

            return new List<Roundtrip>();
        }
    }
}
