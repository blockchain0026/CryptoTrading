using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Events
{
    public class TradeStrategyCreatedDomainEvent:INotification
    {
        public TradeStrategy TradeStrategy { get; private set; }


        public TradeStrategyCreatedDomainEvent(TradeStrategy tradeStrategy)
        {
            TradeStrategy = tradeStrategy;
        }
    }
}
