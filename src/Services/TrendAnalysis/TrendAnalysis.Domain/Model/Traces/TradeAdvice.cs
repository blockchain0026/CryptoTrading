using CryptoTrading.Services.TrendAnalysis.Domain.Exceptions;
using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public class TradeAdvice : Entity
    {
        public string TradeAdviceId { get; private set; }
        public string TraceId { get; private set; }
        public DateTime DateCreated { get; private set; }
        public TradingSignalType TradingSignalType { get; private set; }
        private int _tradingSignalTypeId;

        public decimal TargetPrice { get; private set; }

        public decimal Price { get; private set; }

        protected TradeAdvice()
        {

        }


        public TradeAdvice(string tradeAdviceId, string traceId, DateTime dateCreated, int tradingSignalTypeId, decimal price, decimal targetPrice):this()
        {
            this.TradeAdviceId = tradeAdviceId ?? throw new TrendAnalysisDomainException("The tradeAdviceId is not provided when create new TradeAdvice.");
            this.TraceId = traceId ?? throw new TrendAnalysisDomainException("The traceId is not provided when create new TradeAdvice.");
            this.DateCreated = dateCreated;

            this._tradingSignalTypeId = TradingSignalType.From(tradingSignalTypeId) != null ? tradingSignalTypeId : throw new TrendAnalysisDomainException("The tradingSignalType is not provided when create new TradeAdvice.");
           
            //The line would cause problem with efcore for writing twice to same property.
            //this.TradingSignalType = TradingSignalType.From(tradingSignalTypeId);

            this.Price = price > 0 ? price : throw new TrendAnalysisDomainException("The price must be larger than 0 for TradeAdvice.");

            this.TargetPrice = targetPrice >= price ? targetPrice : throw new TrendAnalysisDomainException("The target price should be large than buy in price.");
        }




    }
}
