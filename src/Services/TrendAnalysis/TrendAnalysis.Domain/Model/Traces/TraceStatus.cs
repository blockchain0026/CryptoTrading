using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public class TraceStatus
      : Enumeration
    {
        public static TraceStatus Prepare = new TraceStatus(1, nameof(Prepare).ToLowerInvariant());
        public static TraceStatus Started = new TraceStatus(2, nameof(Started).ToLowerInvariant());
        public static TraceStatus Closed = new TraceStatus(3, nameof(Closed).ToLowerInvariant());

        protected TraceStatus()
        {
        }

        public TraceStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<TraceStatus> List() =>
            new[] { Prepare, Started, Closed };

        public static TraceStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for TraceStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TraceStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for TraceStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
