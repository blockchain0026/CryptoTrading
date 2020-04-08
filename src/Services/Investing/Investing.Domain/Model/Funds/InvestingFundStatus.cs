using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Funds
{
    public class InvestingFundStatus
      : Enumeration
    {
        public static InvestingFundStatus Initialize = new InvestingFundStatus(1, nameof(Initialize).ToLowerInvariant());
        public static InvestingFundStatus Ended = new InvestingFundStatus(2, nameof(Ended).ToLowerInvariant());
      


        protected InvestingFundStatus()
        {
        }

        public InvestingFundStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<InvestingFundStatus> List() =>
            new[] { Initialize, Ended };

        public static InvestingFundStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for InvestingFundStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static InvestingFundStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for InvestingFundStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
