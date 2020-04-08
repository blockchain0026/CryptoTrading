using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class InvestmentBalanceChangedDomainEvent : INotification
    {
        public InvestmentBalanceChangedDomainEvent(string investmentId, InvestmentType investmentType, Market market, decimal initialBalance, decimal currentBalance)
        {
            InvestmentId = investmentId;
            InvestmentType = investmentType;
            Market = market;
            InitialBalance = initialBalance;
            CurrentBalance = currentBalance;
        }

        public string InvestmentId { get; private set; }
        public InvestmentType InvestmentType { get; private set; }
        public Market Market { get; private set; }
        public decimal InitialBalance { get; private set; }
        public decimal CurrentBalance { get; private set; }
    }
}
