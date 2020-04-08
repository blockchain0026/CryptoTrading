using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public class Investment : ValueObject
    {
        public string InvestmentId { get; private set; }
        public bool IsClosed { get; private set; }

        public Investment(string investmentId, bool isClosed)
        {
            InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
            IsClosed = isClosed;
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.InvestmentId;
            yield return this.IsClosed;
        }
    }
}
