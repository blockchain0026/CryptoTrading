using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{

    public class InvestmentReadyDomainEvent : INotification
    {
        public InvestmentReadyDomainEvent(Investment investment, Trace trace, InvestmentStatus investmentStatus)
        {
            Investment = investment ?? throw new ArgumentNullException(nameof(investment));
            Trace = trace ?? throw new ArgumentNullException(nameof(trace));
            InvestmentStatus = investmentStatus?? throw new ArgumentNullException(nameof(investmentStatus));
        }

        public Investment Investment { get; private set; }
        public Trace Trace { get; private set; }
        public InvestmentStatus InvestmentStatus { get; private set; }

    }
}
