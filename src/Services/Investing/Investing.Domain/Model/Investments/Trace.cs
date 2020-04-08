using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Investments
{

    public class Trace : ValueObject
    {

        public string TraceId { get; private set; }


        public Trace(string traceId = null)
        {
            TraceId = traceId;
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.TraceId;

        }
    }
}
