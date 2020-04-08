using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Events
{
    public class InvestmentSellingAdviceCreatedDomainEvent:INotification
    {
        public InvestmentSellingAdviceCreatedDomainEvent(string investmentId, int roundtripNumber, decimal executePrice, DateTime dateCreated)
        {
            InvestmentId = investmentId;
            RoundtripNumber = roundtripNumber;
            ExecutePrice = executePrice;
            DateCreated = dateCreated;
        }

        public string InvestmentId { get; private set; }
        public int RoundtripNumber { get; private set; }
        public decimal ExecutePrice { get; private set; }
        public DateTime DateCreated { get; private set; }

    }
}
