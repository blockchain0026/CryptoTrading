using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class InvestmentSettledDomainEvent : INotification
    {
        public InvestmentSettledDomainEvent(Investment investment)
        {
            Investment = investment ?? throw new ArgumentNullException(nameof(investment));
        }

        public Investment Investment { get; private set; }


    }
}
