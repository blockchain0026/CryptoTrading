using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies
{
    public class CandlePeriod
         : Enumeration
    {
        public static CandlePeriod OneMinute = new CandlePeriod(1, nameof(OneMinute).ToLowerInvariant());
        public static CandlePeriod FiveMinutes = new CandlePeriod(2, nameof(FiveMinutes).ToLowerInvariant());
        public static CandlePeriod FifteenMinutes = new CandlePeriod(3, nameof(FifteenMinutes).ToLowerInvariant());
        public static CandlePeriod ThirtyMinutes = new CandlePeriod(4, nameof(ThirtyMinutes).ToLowerInvariant());
        public static CandlePeriod OneHour = new CandlePeriod(5, nameof(OneHour).ToLowerInvariant());
        public static CandlePeriod TwoHours = new CandlePeriod(6, nameof(TwoHours).ToLowerInvariant());
        public static CandlePeriod FourHours = new CandlePeriod(7, nameof(FourHours).ToLowerInvariant());
        public static CandlePeriod OneDay = new CandlePeriod(8, nameof(OneDay).ToLowerInvariant());
        public static CandlePeriod OneWeek = new CandlePeriod(9, nameof(OneWeek).ToLowerInvariant());

        protected CandlePeriod()
        {
        }

        public CandlePeriod(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<CandlePeriod> List() =>
            new[] { OneMinute, FiveMinutes, FifteenMinutes, ThirtyMinutes, OneHour, TwoHours, FourHours, OneDay, OneWeek };

        public static CandlePeriod FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for CandlePeriod: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static CandlePeriod From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for CandlePeriod: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
