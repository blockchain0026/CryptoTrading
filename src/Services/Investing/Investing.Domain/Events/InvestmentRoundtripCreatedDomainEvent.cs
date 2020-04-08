using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class InvestmentRoundtripCreatedDomainEvent : INotification
    {
        public InvestmentRoundtripCreatedDomainEvent(string investmentId, int roundtripNumber, DateTime dateCreated, Market market, decimal entryBalance, decimal executePrice, decimal targetPrice, decimal stopLossPrice)
        {
            InvestmentId = investmentId;
            RoundtripNumber = roundtripNumber;
            DateCreated = dateCreated;
            Market = market;
            EntryBalance = entryBalance;
            ExecutePrice = executePrice;
            TargetPrice = targetPrice;
            StopLossPrice = stopLossPrice;
        }

        public string InvestmentId { get; private set; }
        public int RoundtripNumber { get; private set; }
        public DateTime DateCreated { get; private set; }
        public Market Market { get; private set; }
        public decimal EntryBalance { get; private set; }
        public decimal ExecutePrice { get; private set; }
        public decimal TargetPrice { get; private set; }
        public decimal StopLossPrice { get; private set; }

    }
}
