using CryptoTrading.Services.Investing.Domain.Model.Funds;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class InvestmentFundingCanceledDomainEvent : INotification
    {
        public InvestmentFundingCanceledDomainEvent(Fund fund, string investmentId)
        {
            Fund = fund ?? throw new ArgumentNullException(nameof(fund));
            InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
        }

        public Fund Fund { get; private set; }
        public string InvestmentId { get; private set; }

    }
}
