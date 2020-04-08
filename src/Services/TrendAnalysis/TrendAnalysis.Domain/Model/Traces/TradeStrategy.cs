using CryptoTrading.Services.TrendAnalysis.Domain.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Exceptions;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public class TradeStrategy : Entity
    {
        public string StrategyId { get; private set; }
        public string TraceId { get; private set; }

        public Strategy Strategy { get; private set; }
        public int Weight { get; private set; }
        public int WarmUp { get; private set; }
        public TradeSignal TradeSignal { get; private set; }

        protected TradeStrategy()
        {

        }

        public TradeStrategy(string traceId, string strategyId, Strategy strategy, int weight, TradeSignal tradeSignal) : this()
        {
            this.TraceId = traceId ?? throw new TrendAnalysisDomainException("The trace Id is not provided when create new trade strategy.");
            this.StrategyId = strategyId ?? throw new TrendAnalysisDomainException("The strategy Id is not provided when create new trade strategy.");
            this.Strategy = strategy ?? throw new TrendAnalysisDomainException("The strategy is not provided when create new trade strategy.");
            this.Weight = weight > 0 && weight <= 100 ? weight : throw new TrendAnalysisDomainException("The weight must be larger than 0 and smaller than 100 for trade strategy.");
            this.WarmUp = strategy.MinimumAmountOfCandles >= 0 ? strategy.MinimumAmountOfCandles : throw new TrendAnalysisDomainException("The warm up must be equal or larger than 0 for trade strategy.");
            this.TradeSignal = tradeSignal ?? throw new TrendAnalysisDomainException("The trade signal is not provided.");
        }

        public void Analysis(IEnumerable<Candle> candles, IIndicatorService indicatorService)
        {
            var lastCandle = candles.Last();

            if (lastCandle.Timestamp == this.TradeSignal.SignalCandleDateTime)
            {
                return;
            }

            var signalType = this.Strategy.Analysis(candles, indicatorService);
            this.TradeSignal = new TradeSignal(this.TraceId, lastCandle.Timestamp, lastCandle.Close, signalType.Id);
        }

        public void SetWeight(int weight)
        {
            this.Weight = weight > 0 ? weight : throw new TrendAnalysisDomainException("The weight must be larger than 0 for trade strategy.");

            this.AddDomainEvent(new TradeStrategyWeightChangedDomainEvent(
                this.TraceId,
                this.StrategyId,
                this.Strategy.Name,
                this.Weight
                ));
        }

        public CandlePeriod GetIdealPeriod()
        {
            return this.Strategy.CandlePeriod;
        }

        public bool IsUpToDate(ITimeService timeService)
        {
            var now = timeService.GetCurrentDateTime();
            var lastUpdate = this.TradeSignal.SignalCandleDateTime;

            var oneCandleMins = CandlePeriodService.GetOneCandleMinutesByPeriod(this.GetIdealPeriod());

            if ((now - lastUpdate).TotalSeconds > oneCandleMins * 60 * 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
