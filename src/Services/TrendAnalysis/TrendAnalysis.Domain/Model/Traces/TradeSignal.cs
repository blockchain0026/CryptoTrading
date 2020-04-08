using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public class TradeSignal : ValueObject
    {

        public string TraceId { get; private set; }
        public DateTime SignalCandleDateTime { get; private set; }
        public decimal Price { get; private set; }
        public TradingSignalType TradingSignalType { get; private set; }
        private int _tradingSignalTypeId;

        public TradeSignal(string traceId, DateTime signalCandleDateTime, decimal price, int tradingSignalTypeId)
        {
            TraceId = traceId ?? throw new ArgumentNullException(nameof(traceId));
            SignalCandleDateTime = signalCandleDateTime;
            Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            _tradingSignalTypeId = TradingSignalType.From(tradingSignalTypeId) != null ? tradingSignalTypeId : throw new ArgumentOutOfRangeException(nameof(tradingSignalTypeId));
        }

        public TradingSignalType GetSignalType()
        {
            return TradingSignalType.From(this._tradingSignalTypeId);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.TraceId;
            yield return this.SignalCandleDateTime;
            yield return this.Price;
            yield return this.TradingSignalType.Id;
        }
    }
}
