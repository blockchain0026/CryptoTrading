using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Events
{


    public class TradeAdviceCreatedDomainEvent : INotification
    {
        public TradeAdviceCreatedDomainEvent(TradeAdvice tradeAdvice, TradingSignalType tradingSignalType, IEnumerable<Candle> candles)
        {
            TradeAdvice = tradeAdvice ?? throw new ArgumentNullException(nameof(tradeAdvice));
            TradingSignalType = tradingSignalType ?? throw new ArgumentNullException(nameof(tradingSignalType));
            Candles = candles ?? throw new ArgumentNullException(nameof(candles));
        }

        public TradeAdvice TradeAdvice { get; private set; }
        public TradingSignalType TradingSignalType { get; private set; }
        public IEnumerable<Candle> Candles { get; private set; }


    }
}
