using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Investments
{
    public class InvestmentStatus
       : Enumeration
    {
        public static InvestmentStatus Prepare = new InvestmentStatus(1, nameof(Prepare).ToLowerInvariant());
        public static InvestmentStatus Settled = new InvestmentStatus(2, nameof(Settled).ToLowerInvariant());
        public static InvestmentStatus Ready = new InvestmentStatus(3, nameof(Ready).ToLowerInvariant());
        public static InvestmentStatus Started = new InvestmentStatus(4, nameof(Started).ToLowerInvariant());
        public static InvestmentStatus ForcedClosing = new InvestmentStatus(5, nameof(ForcedClosing).ToLowerInvariant());
        public static InvestmentStatus Closed = new InvestmentStatus(6, nameof(Closed).ToLowerInvariant());


        protected InvestmentStatus()
        {
        }

        public InvestmentStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<InvestmentStatus> List() =>
            new[] { Prepare, Settled, Ready, Started, ForcedClosing, Closed };

        public static InvestmentStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for InvestmentStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static InvestmentStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for InvestmentStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
