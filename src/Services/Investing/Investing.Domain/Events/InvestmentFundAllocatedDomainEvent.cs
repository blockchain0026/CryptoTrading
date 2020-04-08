using CryptoTrading.Services.Investing.Domain.Model.Funds;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class InvestmentFundAllocatedDomainEvent : INotification
    {
        public InvestmentFundAllocatedDomainEvent(Fund fund, string investmentId, decimal quantity)
        {
            Fund = fund ?? throw new ArgumentNullException(nameof(fund));
            InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
            Quantity = quantity;
        }

        public Fund Fund { get; private set; }
        public string InvestmentId { get; private set; }
        public decimal Quantity { get; private set; }
    }
}
