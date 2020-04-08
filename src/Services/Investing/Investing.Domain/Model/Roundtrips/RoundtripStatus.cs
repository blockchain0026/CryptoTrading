using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Roundtrips
{

    public class RoundtripStatus
    : Enumeration
    {
        public static RoundtripStatus EntryOrderSubmitted = new RoundtripStatus(1, nameof(EntryOrderSubmitted).ToLowerInvariant());
        public static RoundtripStatus Entry = new RoundtripStatus(2, nameof(Entry).ToLowerInvariant());
        public static RoundtripStatus ExitOrderSubmitted = new RoundtripStatus(3, nameof(ExitOrderSubmitted).ToLowerInvariant());
        public static RoundtripStatus Exit = new RoundtripStatus(4, nameof(Exit).ToLowerInvariant());
        public static RoundtripStatus ForceSell = new RoundtripStatus(5, nameof(ForceSell).ToLowerInvariant());
        public static RoundtripStatus ForceExit = new RoundtripStatus(6, nameof(ForceExit).ToLowerInvariant());

        protected RoundtripStatus()
        {
        }

        public RoundtripStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<RoundtripStatus> List() =>
            new[] { EntryOrderSubmitted, Entry, ExitOrderSubmitted, Exit , ForceSell, ForceExit };

        public static RoundtripStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for RoundtripStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static RoundtripStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for RoundtripStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
