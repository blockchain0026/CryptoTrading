using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTrading.Services.Investing.Domain.Model.Investments
{
    public class InvestmentType
     : Enumeration
    {
        public static InvestmentType Live = new InvestmentType(1, nameof(Live).ToLowerInvariant());
        public static InvestmentType Paper = new InvestmentType(2, nameof(Paper).ToLowerInvariant());
        public static InvestmentType Backtesting = new InvestmentType(3, nameof(Backtesting).ToLowerInvariant());


        protected InvestmentType()
        {
        }

        public InvestmentType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<InvestmentType> List() =>
            new[] { Live, Paper, Backtesting };

        public static InvestmentType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for InvestmentType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static InvestmentType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for InvestmentType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}