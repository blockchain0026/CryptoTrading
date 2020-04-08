using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Exceptions
{
    public class TrendAnalysisDomainException : Exception
    {
        public TrendAnalysisDomainException()
        { }

        public TrendAnalysisDomainException(string message)
            : base(message)
        { }

        public TrendAnalysisDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
