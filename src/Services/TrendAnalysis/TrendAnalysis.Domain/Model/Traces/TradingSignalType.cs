using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public class TradingSignalType
       : Enumeration
    {
        public static TradingSignalType Hold = new TradingSignalType(1, nameof(Hold).ToLowerInvariant());
        public static TradingSignalType Buy = new TradingSignalType(2, nameof(Buy).ToLowerInvariant());
        public static TradingSignalType Sell = new TradingSignalType(3, nameof(Sell).ToLowerInvariant());

        protected TradingSignalType()
        {
        }

        public TradingSignalType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<TradingSignalType> List() =>
            new[] { Hold, Buy, Sell };

        public static TradingSignalType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for TradingSignalType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TradingSignalType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for TradingSignalType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
