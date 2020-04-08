using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Exceptions
{
    public class InvestingDomainException : Exception
    {
        public InvestingDomainException()
        { }

        public InvestingDomainException(string message)
            : base(message)
        { }

        public InvestingDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
