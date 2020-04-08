using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class ExchangeAccessDomainException : Exception
    {
        public ExchangeAccessDomainException()
        { }

        public ExchangeAccessDomainException(string message)
            : base(message)
        { }

        public ExchangeAccessDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
