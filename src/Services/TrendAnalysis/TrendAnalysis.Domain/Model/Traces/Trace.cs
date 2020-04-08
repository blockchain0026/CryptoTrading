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
    public class Trace : Entity, IAggregateRoot
    {
        public string TraceId { get; private set; }
        public Investment Investment { get; private set; }
        public Market Market { get; private set; }
        public TraceStatus TraceStatus { get; private set; }
        private int _traceStatusId;
        public DateTime? DateStarted { get; private set; }
        public DateTime? DateClosed { get; private set; }

        //for calculating target price.
        public string IdealCandlePeriod { get; private set; }

        private readonly List<TradeStrategy> _tradeStrategies;
        public IReadOnlyCollection<TradeStrategy> TradeStrategies => _tradeStrategies;

        private readonly List<TradeAdvice> _tradeAdvices;
        public IReadOnlyCollection<TradeAdvice> TradeAdvices => _tradeAdvices;


        protected Trace()
        {
            this._tradeStrategies = new List<TradeStrategy>();
            this._tradeAdvices = new List<TradeAdvice>();
        }

        public Trace(string investmentId, int exchangeId, string baseCurrency, string quoteCurrency) : this()
        {
            this.TraceId = Guid.NewGuid().ToString();
            this.Investment = investmentId != null ?
                new Investment(investmentId, false) :
                throw new TrendAnalysisDomainException("The investmentId is not provided when create new Trace.");
            this.Market = baseCurrency != null && quoteCurrency != null ?
                new Market(exchangeId, baseCurrency, quoteCurrency) :
                throw new TrendAnalysisDomainException("The baseCurrency and quoteCurrency must be provided when create new Trace.");
            this._traceStatusId = TraceStatus.Prepare.Id;
            this.IdealCandlePeriod = string.Empty;

            this.AddDomainEvent(
                new TraceCreatedDomainEvent(this));
        }



        public void StartTracing(ITimeService timeService, CandlePeriod idealCandlePeriod = null)
        {
            if (timeService == null)
            {
                throw new TrendAnalysisDomainException("Time service is not provided when operating on trace.");
            }

            if (idealCandlePeriod == null)
            {
                idealCandlePeriod = this.CalculateIdealCandlePeriod();
                this.IdealCandlePeriod = idealCandlePeriod.Name;
            }
            else
            {
                this.IdealCandlePeriod = idealCandlePeriod.Name;
            }

            if (this.TraceStatus.Id == TraceStatus.Closed.Id)
            {
                throw new TrendAnalysisDomainException("Cannot start tracing after closed.");
            }
            if (this.TraceStatus.Id != TraceStatus.Started.Id)
            {
                if (!this.TradeStrategies.Any())
                {
                    throw new TrendAnalysisDomainException("There must be at least one strategy before starting trace.");
                }
                this.DateStarted = timeService.GetCurrentDateTime();
                this._traceStatusId = TraceStatus.Started.Id;

                this.AddDomainEvent(new TraceStartedDomainEvent(
                    this.TraceId,
                    CandlePeriod.FromName(this.IdealCandlePeriod),
                    this.TraceStatus,
                    this.DateStarted ?? throw new TrendAnalysisDomainException("DateStarted missing when changing status.")));
            }
        }

        public void CloseTracing(ITimeService timeService)
        {
            if (timeService == null)
            {
                throw new TrendAnalysisDomainException("Time service is not provided when operating on trace.");
            }

            if (this.TraceStatus.Id == TraceStatus.Prepare.Id)
            {
                throw new TrendAnalysisDomainException("Cannot close tracing before started");
            }

            if (this.TraceStatus.Id != TraceStatus.Closed.Id)
            {
                this.DateClosed = timeService.GetCurrentDateTime();
                this._traceStatusId = TraceStatus.Closed.Id;

                this.AddDomainEvent(new TraceClosedDomainEvent(
                    this.TraceId,
                    this.TraceStatus,
                    this.DateClosed ?? throw new TrendAnalysisDomainException("DateStarted missing when changing status.")
                    ));
            }
        }

        public void CandleUpdated(IEnumerable<Candle> candles, CandlePeriod candlePeriod, ITimeService timeService, IIndicatorService indicatorService, string strategyId = null)
        {
            if (timeService == null)
            {
                throw new TrendAnalysisDomainException("Time service is not provided when operating on trace.");
            }

            if (indicatorService == null)
            {
                throw new TrendAnalysisDomainException("Indicator service is not provided when operating on trace.");
            }

            if (this.TraceStatus.Id != TraceStatus.Started.Id)
            {
                throw new TrendAnalysisDomainException("Updating candles can only happened before closed and after started.");
            }

            if (this.DateStarted == null)
            {
                throw new TrendAnalysisDomainException(nameof(this.DateStarted));
            }



            var signalUpdated = false;

            if (strategyId != null)
            {
                var strategy = this._tradeStrategies.Where(s => s.StrategyId == strategyId).SingleOrDefault();
                if (strategy == null)
                {
                    throw new TrendAnalysisDomainException("Strategy to update not found");
                }

                if (!this.HasCompleteCandlesBetween(
                      candles,
                      candlePeriod,
                      this.CalculateCandleStartDateWithWarmUp(candlePeriod, strategy.WarmUp),
                      timeService.GetCurrentDateTime()
                      ))
                {
                    throw new TrendAnalysisDomainException("The candles data provided are not completed, each candles from started date to current date must be provided.");
                }

                var orignalSignal = strategy.TradeSignal;

                strategy.Analysis(candles, indicatorService);

                if (strategy.TradeSignal.GetSignalType().Id != orignalSignal.TradingSignalType.Id)
                {
                    signalUpdated = true;
                }
            }
            else
            {
                var matchedStrategies = this._tradeStrategies.Where(s => s.GetIdealPeriod().Id == candlePeriod.Id);
                foreach (var strategy in matchedStrategies)
                {

                    if (!this.HasCompleteCandlesBetween(
                        candles,
                        candlePeriod,
                        this.CalculateCandleStartDateWithWarmUp(candlePeriod, strategy.WarmUp),
                        timeService.GetCurrentDateTime()
                        ))
                    {
                        throw new TrendAnalysisDomainException("The candles data provided are not completed, each candles from started date to current date must be provided.");
                    }

                    var orignalSignal = strategy.TradeSignal;

                    strategy.Analysis(candles, indicatorService);

                    if (strategy.TradeSignal.GetSignalType().Id != orignalSignal.TradingSignalType.Id)
                    {
                        signalUpdated = true;
                    }
                }
            }


            if (signalUpdated == true)
            {
                this.AddDomainEvent(
                    new TradeSignalUpdatedDomainEvent(this, candles, timeService)
                    );
            }

            //var currentCandleTime=
        }

        public void AddStrategy(Strategy strategy, ITimeService timeService, int weight = 50)
        {
            if (this._traceStatusId != TraceStatus.Prepare.Id)
            {
                throw new TrendAnalysisDomainException("Adding Strategy can only happened in prepare status.");
            }
            if (timeService == null)
            {
                throw new TrendAnalysisDomainException("Time service is not provided when operating on trace.");
            }
            if (this._tradeStrategies.Where(t => t.Strategy == strategy).SingleOrDefault() != null)
            {
                throw new TrendAnalysisDomainException("Could not at the same strategy twice.");
            }


            var currentCandleDateTime = strategy.GetCurrentCandleDateTime(timeService);
            var signal = new TradeSignal(this.TraceId, currentCandleDateTime, 1, TradingSignalType.Hold.Id); ;

            var strategyToAdd = new TradeStrategy(
                this.TraceId,
                Guid.NewGuid().ToString(),
                strategy ?? throw new TrendAnalysisDomainException("The strategy is not provided when create new trade strategy."),
                weight,
                signal
                );

            this._tradeStrategies.Add(strategyToAdd);
        }

        public void BuildAdvice(IEnumerable<Candle> candles, decimal currentPrice, ITimeService timeService)
        {
            if (this._traceStatusId != TraceStatus.Started.Id)
            {
                throw new TrendAnalysisDomainException("Building Advice can only happened after started.");
            }
            if (this._tradeStrategies.Where(t => t.IsUpToDate(timeService) == false).Any())
            {
                return;
            }

            var decision = this.CalculateDecisionByWeight();

            TradeAdvice tradeAdvice = null;


            if (decision.Id == TradingSignalType.Buy.Id)
            {

                var targetPrice = CalculateTargetPrice(candles, currentPrice);

                var advice = new TradeAdvice(
                    Guid.NewGuid().ToString(),
                    this.TraceId,
                    timeService.GetCurrentDateTime(),
                    decision.Id,
                    currentPrice,
                    targetPrice
                    );

                tradeAdvice = advice;

                /*var targetIndex = 0;

                for (int index = 0; index < (fibonacci.Count - 1); index++)
                {
                    if (fibonacci[index].Value == top)
                    {
                        targetIndex = index + 1;
                    }
                }*/
            }

            if (decision.Id == TradingSignalType.Hold.Id)
            {

                var advice = new TradeAdvice(
                Guid.NewGuid().ToString(),
                this.TraceId,
                timeService.GetCurrentDateTime(),
                decision.Id,
                currentPrice,
                currentPrice
                );

                tradeAdvice = advice;
            }

            if (decision.Id == TradingSignalType.Sell.Id)
            {

                var advice = new TradeAdvice(
                Guid.NewGuid().ToString(),
                this.TraceId,
                timeService.GetCurrentDateTime(),
                decision.Id,
                currentPrice,
                currentPrice
                );

                tradeAdvice = advice;
            }


            this._tradeAdvices.Add(tradeAdvice);

            this.AddDomainEvent(new TradeAdviceCreatedDomainEvent(tradeAdvice, decision, candles));
        }

        public static decimal CalculateTargetPrice(IEnumerable<Candle> candles, decimal currentPrice)
        {
            var fibonacci = CalculateFibonacci(candles);
            var orderedFibs = fibonacci.OrderBy(f => f.Value);

            //var currentPrice = candles.Last().Close;

            decimal top = orderedFibs.Last().Value;
            decimal bottom = orderedFibs.First().Value;

            foreach (var fib in fibonacci)
            {
                if (currentPrice >= fib.Value)
                {

                    if (fib.Value > bottom)
                    {
                        bottom = fib.Value;
                    }
                }
                else
                {
                    if (fib.Value < top)
                    {
                        top = fib.Value;
                    }
                }
            }

            var targetPrice = top;

            return targetPrice;
        }



        #region Private Functions
        private bool HasCompleteCandlesBetween(IEnumerable<Candle> candles, CandlePeriod period, DateTime from, DateTime to)
        {

            var sorted = candles.OrderBy(c => c.Timestamp);


            var minute = this.GetPeriodMinute(period);
            var hour = this.GetPeriodHour(period);
            var day = this.GetPeriodDay(period);
            var week = this.GetPeriodWeek(period);


            //Calculate first and last candle's datetime depends on candle period. 
            DateTime start = from;
            DateTime end = to;



            if (minute == 0 && hour == 0 && day == 0)
            {
                DateTime monday = from;
                while (monday.DayOfWeek != DayOfWeek.Monday)
                {
                    monday = monday.AddDays(1);
                }

                start = new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, 0);

                start = start.AddDays(-7);

                monday = to;

                while (monday.DayOfWeek != DayOfWeek.Monday)
                {
                    monday = monday.AddDays(-1);
                }


                end = new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, 0);
                end = end.AddDays(-7);
            }
            else if (minute == 0 && hour == 0)
            {
                if (from.Hour != 0 || from.Minute != 0 || from.Second != 0 || from.Millisecond != 0)
                {
                    var addDays = from.AddDays(1);
                    start = new DateTime(addDays.Year, addDays.Month, addDays.Day, 0, 0, 0, 0);
                    start = start.AddDays(-1);

                    var substractDays = to.AddDays(-1);
                    end = new DateTime(substractDays.Year, substractDays.Month, substractDays.Day, 0, 0, 0, 0);

                }
                else
                {
                    start = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0);
                    start = start.AddDays(-1);
                    end = new DateTime(to.Year, to.Month, to.Day, 0, 0, 0, 0);
                    end = end.AddDays(-1);
                }
            }
            else if (minute == 0)
            {
                if (from.Hour % hour != 0
                    || from.Minute != 0 || from.Second != 0 || from.Millisecond != 0)
                {
                    int hoursToAdd = hour - (from.Hour % hour);
                    var addHours = from.AddHours(hoursToAdd);
                    start = new DateTime(addHours.Year, addHours.Month, addHours.Day, addHours.Hour, 0, 0, 0);

                    start = start.AddHours(-hour);


                    int hoursToSub = to.Hour % hour;

                    //Current candle's timestamp.
                    var subHours = to.AddHours(-hoursToSub);
                    end = new DateTime(subHours.Year, subHours.Month, subHours.Day, subHours.Hour, 0, 0, 0);

                    //Exclude unfinished candle.
                    end = end.AddHours(-hour);
                }
                else
                {
                    start = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0, 0);
                    start = start.AddHours(-hour);
                    end = new DateTime(to.Year, to.Month, to.Day, to.Hour, 0, 0, 0);
                    end = end.AddHours(-hour);
                }
            }
            else
            {
                if (from.Minute % minute != 0
                    || from.Second != 0 || from.Millisecond != 0)
                {
                    int minutesToAdd = minute - (from.Minute % minute);
                    var addMinutes = from.AddMinutes(minutesToAdd);
                    start = new DateTime(addMinutes.Year, addMinutes.Month, addMinutes.Day, addMinutes.Hour, addMinutes.Minute, 0, 0);
                    start = start.AddMinutes(-minute);

                    int minutesToSub = to.Minute % minute;
                    var subMinutes = to.AddMinutes(-minutesToSub);
                    end = new DateTime(subMinutes.Year, subMinutes.Month, subMinutes.Day, subMinutes.Hour, subMinutes.Minute, 0, 0);
                    end = end.AddMinutes(-minute);
                }
                else
                {
                    start = new DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0, 0);
                    start = start.AddMinutes(-minute);
                    end = new DateTime(to.Year, to.Month, to.Day, to.Hour, to.Minute, 0, 0);
                    end = end.AddMinutes(-minute);
                }
            }

            DateTime toMatch = start;


            if (to < end)
            {
                if (sorted.Any())
                {
                    throw new TrendAnalysisDomainException("The candles data is out of range, only need the candles from start date to end date.");
                }
                return true;
            }


            foreach (var candle in sorted)
            {
                if (candle.Timestamp != toMatch)
                    return false;

                if (minute != 0)
                {
                    toMatch = toMatch.AddMinutes(minute);
                }
                else if (hour != 0)
                {
                    toMatch = toMatch.AddHours(hour);
                }
                else if (day != 0)
                {
                    toMatch = toMatch.AddDays(day);
                }
                else if (week != 0)
                {
                    toMatch = toMatch.AddDays(7);
                }
            }

            if (!sorted.Any())
            {
                return false;
            }

            if (sorted.Last().Timestamp != end)
            {
                if (sorted.Last().Timestamp > end)
                {
                    throw new TrendAnalysisDomainException("The candles data is out of range, only need the candles from start date to end date.");
                }

                return false;
            }

            return true;
        }

        private void CheckCandleTimestamp(CandlePeriod candlePeriod, DateTime timestamp)
        {
            var period = candlePeriod.Id;
            if (period == CandlePeriod.OneMinute.Id || period == CandlePeriod.FiveMinutes.Id || period == CandlePeriod.FifteenMinutes.Id || period == CandlePeriod.ThirtyMinutes.Id)
            {
                //Seconds and MilliSeconds should be zero.
                if (timestamp.Second != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"Second\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Millisecond != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"MilliSecond\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }
            }
            else if (period == CandlePeriod.OneHour.Id || period == CandlePeriod.TwoHours.Id || period == CandlePeriod.FourHours.Id)
            {
                //Minutes and Seconds and MilliSeconds should be zero.
                if (timestamp.Minute != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"Minute\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Second != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"Second\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Millisecond != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"MilliSecond\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }
            }
            else if (period == CandlePeriod.OneDay.Id)
            {
                //Hours and Minutes and Seconds and MilliSeconds should be zero.
                if (timestamp.Hour != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"Hour\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Minute != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"Minute\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Second != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"Second\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Millisecond != 0)
                {
                    throw new TrendAnalysisDomainException($"The \"MilliSecond\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }
            }


            if (period == CandlePeriod.OneMinute.Id)
            {
            }
            else if (period == CandlePeriod.FiveMinutes.Id)
            {
                if (timestamp.Minute % 5 != 0)
                {
                    throw new TrendAnalysisDomainException("The minute property of timestamp must be divisible by 5 on 5m chart.");
                }
            }
            else if (period == CandlePeriod.FifteenMinutes.Id)
            {
                if (timestamp.Minute % 15 != 0)
                {
                    throw new TrendAnalysisDomainException("The minute property of timestamp must be divisible by 15 on 15m chart.");
                }
            }
            else if (period == CandlePeriod.ThirtyMinutes.Id)
            {
                if (timestamp.Minute % 30 != 0)
                {
                    throw new TrendAnalysisDomainException("The minute property of timestamp must be divisible by 30 on 30m chart.");
                }
            }
            else if (period == CandlePeriod.OneHour.Id)
            {
            }
            else if (period == CandlePeriod.TwoHours.Id)
            {
                if (timestamp.Hour % 2 != 0)
                {
                    throw new TrendAnalysisDomainException("The hour property of timestamp must be divisible by 2 on 2hr chart.");
                }
            }
            else if (period == CandlePeriod.FourHours.Id)
            {
                if (timestamp.Hour % 4 != 0)
                {
                    throw new TrendAnalysisDomainException("The hour property of timestamp must be divisible by 4 on 4hr chart.");
                }
            }
            else if (period == CandlePeriod.OneDay.Id)
            {
            }
            else if (period == CandlePeriod.OneWeek.Id)
            {
                if (timestamp.DayOfWeek != DayOfWeek.Monday)
                {
                    throw new TrendAnalysisDomainException("The DayOfWeek property of timestamp must be equal to Monday.");
                }
            }
        }

        private int GetPeriodMinute(CandlePeriod candlePeriod)
        {
            var period = candlePeriod.Id;
            int minutes = 0;

            if (period == CandlePeriod.OneMinute.Id)
            {
                minutes = 1;
            }
            else if (period == CandlePeriod.FiveMinutes.Id)
            {
                minutes = 5;
            }
            else if (period == CandlePeriod.FifteenMinutes.Id)
            {
                minutes = 15;
            }
            else if (period == CandlePeriod.ThirtyMinutes.Id)
            {
                minutes = 30;
            }

            return minutes;
        }

        private int GetPeriodHour(CandlePeriod candlePeriod)
        {
            var period = candlePeriod.Id;
            int hours = 0;

            if (period == CandlePeriod.OneHour.Id)
            {
                hours = 1;
            }
            else if (period == CandlePeriod.TwoHours.Id)
            {
                hours = 2;
            }
            else if (period == CandlePeriod.FourHours.Id)
            {
                hours = 4;
            }


            return hours;
        }

        private int GetPeriodDay(CandlePeriod candlePeriod)
        {
            var period = candlePeriod.Id;
            int days = 0;

            if (period == CandlePeriod.OneDay.Id)
            {
                days = 1;
            }

            return days;
        }

        private int GetPeriodWeek(CandlePeriod candlePeriod)
        {
            var period = candlePeriod.Id;
            int weeks = 0;

            if (period == CandlePeriod.OneWeek.Id)
            {
                weeks = 1;
            }

            return weeks;
        }


        private static List<KeyValuePair<double, decimal>> CalculateFibonacci(IEnumerable<Candle> candles)
        {
            var result = new List<KeyValuePair<double, decimal>>();

            //Determine it's a downtrend or uptrend.
            //var trend = candles.TrendDetermine();

            Candle highestPrice = candles.First();
            Candle lowestPrice = candles.First();

            foreach (var candle in candles)
            {
                if (candle.High > highestPrice.High)
                {
                    highestPrice = candle;
                }

                if (candle.Low < lowestPrice.Low)
                {
                    lowestPrice = candle;
                }
            }


            decimal start;
            decimal end;
            decimal body;

            if (highestPrice.Timestamp < lowestPrice.Timestamp)
            {
                start = highestPrice.High;
                end = lowestPrice.Low;
            }
            else if (highestPrice.Timestamp > lowestPrice.Timestamp)
            {
                start = lowestPrice.Low;
                end = highestPrice.High;
            }
            else
            {
                //throw new TrendAnalysisDomainException("The data is not enough to calculate fibonacci.");
                start = lowestPrice.Low;
                end = highestPrice.High;
            }

            body = start - end;

            //0
            result.Add(
                new KeyValuePair<double, decimal>(0, end));
            //0.236
            result.Add(
                new KeyValuePair<double, decimal>(0.236, end + (body * 0.236M)));
            //0.382
            result.Add(
                new KeyValuePair<double, decimal>(0.382, end + (body * 0.382M)));
            //0.5
            result.Add(
                new KeyValuePair<double, decimal>(0.5, end + (body * 0.5M)));
            //0.618
            result.Add(
                new KeyValuePair<double, decimal>(0.618, end + (body * 0.618M)));
            //0.786
            result.Add(
                new KeyValuePair<double, decimal>(0.786, end + (body * 0.786M)));
            //1
            result.Add(
                new KeyValuePair<double, decimal>(0, end + body));

            return result;
        }

        private TradingSignalType CalculateDecisionByWeight()
        {
            var strategies = this._tradeStrategies;

            int hold = 0;
            int buy = 0;
            int sell = 0;

            foreach (var strategy in strategies)
            {
                if (strategy.TradeSignal.GetSignalType().Id == TradingSignalType.Hold.Id)
                {
                    hold += strategy.Weight;
                }
                else if (strategy.TradeSignal.GetSignalType().Id == TradingSignalType.Buy.Id)
                {
                    buy += strategy.Weight;
                }
                else if (strategy.TradeSignal.GetSignalType().Id == TradingSignalType.Sell.Id)
                {
                    sell += strategy.Weight;
                }
                else
                {
                    throw new TrendAnalysisDomainException("TradingSignalType is missing when calculate decision by weight.");
                }
            }

            if (buy > hold && buy > sell)
            {
                return TradingSignalType.Buy;
            }
            else if (sell > hold && sell > buy)
            {
                return TradingSignalType.Sell;
            }
            else
            {
                return TradingSignalType.Hold;
            }
        }

        private CandlePeriod CalculateIdealCandlePeriod()
        {
            int oneMin = 0;
            int fiveMins = 0;
            int fifteenMins = 0;
            int thirtyMins = 0;
            int oneHour = 0;
            int twoHours = 0;
            int fourHours = 0;
            int oneDay = 0;
            int oneWeek = 0;

            foreach (var strategy in this._tradeStrategies)
            {
                var min = GetPeriodMinute(strategy.GetIdealPeriod());
                var hour = GetPeriodHour(strategy.GetIdealPeriod());
                var day = GetPeriodDay(strategy.GetIdealPeriod());
                var week = GetPeriodWeek(strategy.GetIdealPeriod());

                if (min != 0)
                {
                    if (min == 1)
                    {
                        oneMin += strategy.Weight;
                    }
                    else if (min == 5)
                    {
                        fiveMins += strategy.Weight;
                    }
                    else if (min == 15)
                    {
                        fifteenMins += strategy.Weight;
                    }
                    else if (min == 30)
                    {
                        thirtyMins += strategy.Weight;
                    }
                }
                else if (hour != 0)
                {
                    if (hour == 1)
                    {
                        oneHour += strategy.Weight;
                    }
                    else if (hour == 2)
                    {
                        twoHours += strategy.Weight;
                    }
                    else if (hour == 4)
                    {
                        fourHours += strategy.Weight;
                    }
                }
                else if (day != 0)
                {
                    if (day == 1)
                    {
                        oneDay += strategy.Weight;
                    }
                }
                else if (week != 0)
                {
                    if (week == 1)
                    {
                        oneWeek += strategy.Weight;
                    }
                }
            }

            var dic = new Dictionary<string, int>();
            dic.Add(CandlePeriod.OneMinute.Name, oneMin);
            dic.Add(CandlePeriod.FiveMinutes.Name, fiveMins);
            dic.Add(CandlePeriod.FifteenMinutes.Name, fifteenMins);
            dic.Add(CandlePeriod.ThirtyMinutes.Name, thirtyMins);
            dic.Add(CandlePeriod.OneHour.Name, oneHour);
            dic.Add(CandlePeriod.TwoHours.Name, twoHours);
            dic.Add(CandlePeriod.FourHours.Name, fourHours);
            dic.Add(CandlePeriod.OneDay.Name, oneDay);
            dic.Add(CandlePeriod.OneWeek.Name, oneWeek);

            var orderedDic = dic.OrderBy(p => p.Value);

            var largest = orderedDic.Last();

            
            return CandlePeriod.FromName(largest.Key);
        }

        private DateTime CalculateCandleStartDateWithWarmUp(CandlePeriod candlePeriod, int warmUp)
        {
            DateTime result = this.DateStarted ?? throw new TrendAnalysisDomainException(nameof(this.DateStarted));

            if (candlePeriod.Id == CandlePeriod.OneMinute.Id)
            {
                result = result.AddMinutes(-warmUp * 1);
            }
            else if (candlePeriod.Id == CandlePeriod.FiveMinutes.Id)
            {
                result = result.AddMinutes(-warmUp * 5);
            }
            else if (candlePeriod.Id == CandlePeriod.FifteenMinutes.Id)
            {
                result = result.AddMinutes(-warmUp * 15);
            }
            else if (candlePeriod.Id == CandlePeriod.ThirtyMinutes.Id)
            {
                result = result.AddMinutes(-warmUp * 30);
            }
            else if (candlePeriod.Id == CandlePeriod.OneHour.Id)
            {
                result = result.AddHours(-warmUp * 1);
            }
            else if (candlePeriod.Id == CandlePeriod.TwoHours.Id)
            {
                result = result.AddHours(-warmUp * 2);
            }
            else if (candlePeriod.Id == CandlePeriod.FourHours.Id)
            {
                result = result.AddHours(-warmUp * 4);
            }
            else if (candlePeriod.Id == CandlePeriod.OneDay.Id)
            {
                result = result.AddDays(-warmUp * 1);
            }
            else if (candlePeriod.Id == CandlePeriod.OneWeek.Id)
            {
                result = result.AddDays(-warmUp * 7);
            }
            else
            {
                throw new TrendAnalysisDomainException("No matching canlde period when calculating candle start date.");
            }

            return result;
        }
        #endregion
    }
}
