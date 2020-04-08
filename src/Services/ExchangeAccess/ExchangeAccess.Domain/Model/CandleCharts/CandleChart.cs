using CryptoTrading.Services.ExchangeAccess.Domain.Exceptions;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts
{
    public class CandleChart : Entity, IAggregateRoot
    {
        #region Properties & Fields

        public string CandleChartId { get; private set; }
        public string MarketId { get; private set; }
        public CandlePeriod CandlePeriod { get; private set; }
        private int _candlePeriodId;
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public int ExchangeId { get; private set; }



        private readonly List<Candle> _candles;
        public IReadOnlyCollection<Candle> Candles => _candles;

        #endregion



        #region Constructor
        protected CandleChart()
        {
            this._candles = new List<Candle>();
        }

        private CandleChart(string candleChartId, string marketId, int candlePeriodId, string baseCurrency, string quoteCurrency, int exchangeId) : this()
        {
            this.CandleChartId = candleChartId ?? throw new ArgumentNullException(nameof(candleChartId));
            this.MarketId = marketId ?? throw new ArgumentNullException(nameof(marketId));
            this._candlePeriodId = candlePeriodId;
            this.BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            this.QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            this.ExchangeId = exchangeId;
        }

        #endregion



        #region Functions

        public static CandleChart FromMarket(Market market, CandlePeriod candlePeriod)
        {
            return new CandleChart(
                Guid.NewGuid().ToString(),
                market.MarketId ?? throw new ExchangeAccessDomainException("The Market Id isn't provided."),
                candlePeriod != null ? candlePeriod.Id : throw new ExchangeAccessDomainException("The candle period is not provided."),
                market.BaseCurrency ?? throw new ExchangeAccessDomainException("The base currency is not provided."),
                market.QuoteCurrency ?? throw new ExchangeAccessDomainException("The quote currency is not provided."),
                market.Exchange != null ? market.Exchange.ExchangeId : throw new ExchangeAccessDomainException("The exchange is not provided.")
                );
        }


        public void UpdateCandle(DateTime timestamp, decimal high, decimal low, decimal open, decimal close, decimal volume)
        {
            this.CheckCandleTimestamp(timestamp);

            var existingCandle = this._candles.Where(c => c.Timestamp == timestamp).SingleOrDefault();
            if (existingCandle != null)
            {
                existingCandle.UpdateData(high, low, open, close, volume);
            }
            else
            {
                var candle = new Candle(timestamp, high, low, open, close, volume);
                this._candles.Add(candle);
            }
        }

        public IEnumerable<Candle> GetCandles(DateTime from, DateTime to)
        {
            var candles = this._candles.Where(c => c.Timestamp >= from && c.Timestamp <= to);

            var sorted = candles.OrderBy(c => c.Timestamp);

            return sorted;
        }

        public bool HasCompleteCandlesBetween(DateTime from, DateTime to)
        {
            if (from > to)
            {
                throw new ExchangeAccessDomainException($"The 'from' argument should be early than 'to' argument. given from:{from.ToString()}, given to:{to.ToString()}.");
            }

            var candles = this._candles.Where(c => c.Timestamp >= from && c.Timestamp <= to);
            var sorted = candles.OrderBy(c => c.Timestamp);


            var minute = this.GetPeriodMinute();
            var hour = this.GetPeriodHour();
            var day = this.GetPeriodDay();
            var week = this.GetPeriodWeek();


            //Calculate first and last candle's datetime depends on this candle chart's period. 
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


                monday = to;

                while (monday.DayOfWeek != DayOfWeek.Monday)
                {
                    monday = monday.AddDays(-1);
                }

                end = new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, 0);
            }
            else if (minute == 0 && hour == 0)
            {
                if (from.Hour != 0 || from.Minute != 0 || from.Second != 0 || from.Millisecond != 0)
                {
                    var addDays = from.AddDays(1);
                    start = new DateTime(addDays.Year, addDays.Month, addDays.Day, 0, 0, 0, 0);

                    var substractDays = to.AddDays(-1);
                    end = new DateTime(substractDays.Year, substractDays.Month, substractDays.Day, 0, 0, 0, 0);
                }
                else
                {
                    start = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0);
                    end = new DateTime(to.Year, to.Month, to.Day, 0, 0, 0, 0);
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

                    int hoursToSub = to.Hour % hour;
                    var subHours = to.AddHours(-hoursToSub);
                    end = new DateTime(subHours.Year, subHours.Month, subHours.Day, subHours.Hour, 0, 0, 0);
                }
                else
                {
                    start = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0, 0);
                    end = new DateTime(to.Year, to.Month, to.Day, to.Hour, 0, 0, 0);
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

                    int minutesToSub = to.Minute % minute;
                    var subMinutes = to.AddMinutes(-minutesToSub);
                    end = start = new DateTime(subMinutes.Year, subMinutes.Month, subMinutes.Day, subMinutes.Hour, subMinutes.Minute, 0, 0);
                }
                else
                {
                    start = new DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0, 0);
                    end = new DateTime(to.Year, to.Month, to.Day, to.Hour, to.Minute, 0, 0);
                }
            }

            DateTime toMatch = start;


            if (start > to && end < from)
            {
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
                return false;
            }

            return true;
        }
        #endregion



        #region Private Functions

        private void CheckCandleTimestamp(DateTime timestamp)
        {
            var period = this._candlePeriodId;
            if (period == CandlePeriod.OneMinute.Id || period == CandlePeriod.FiveMinutes.Id || period == CandlePeriod.FifteenMinutes.Id || period == CandlePeriod.ThirtyMinutes.Id)
            {
                //Seconds and MilliSeconds should be zero.
                if (timestamp.Second != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"Second\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Millisecond != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"MilliSecond\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }
            }
            else if (period == CandlePeriod.OneHour.Id || period == CandlePeriod.TwoHours.Id || period == CandlePeriod.FourHours.Id)
            {
                //Minutes and Seconds and MilliSeconds should be zero.
                if (timestamp.Minute != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"Minute\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Second != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"Second\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Millisecond != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"MilliSecond\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }
            }
            else if (period == CandlePeriod.OneDay.Id)
            {
                //Hours and Minutes and Seconds and MilliSeconds should be zero.
                if (timestamp.Hour != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"Hour\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Minute != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"Minute\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Second != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"Second\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }

                if (timestamp.Millisecond != 0)
                {
                    throw new ExchangeAccessDomainException($"The \"MilliSecond\" property in timestamp for the given candle should be zero on {CandlePeriod.From(period)} chart.");
                }
            }


            if (period == CandlePeriod.OneMinute.Id)
            {
            }
            else if (period == CandlePeriod.FiveMinutes.Id)
            {
                if (timestamp.Minute % 5 != 0)
                {
                    throw new ExchangeAccessDomainException("The minute property of timestamp must be divisible by 5 on 5m chart.");
                }
            }
            else if (period == CandlePeriod.FifteenMinutes.Id)
            {
                if (timestamp.Minute % 15 != 0)
                {
                    throw new ExchangeAccessDomainException("The minute property of timestamp must be divisible by 15 on 15m chart.");
                }
            }
            else if (period == CandlePeriod.ThirtyMinutes.Id)
            {
                if (timestamp.Minute % 30 != 0)
                {
                    throw new ExchangeAccessDomainException("The minute property of timestamp must be divisible by 30 on 30m chart.");
                }
            }
            else if (period == CandlePeriod.OneHour.Id)
            {
            }
            else if (period == CandlePeriod.TwoHours.Id)
            {
                if (timestamp.Hour % 2 != 0)
                {
                    throw new ExchangeAccessDomainException("The hour property of timestamp must be divisible by 2 on 2hr chart.");
                }
            }
            else if (period == CandlePeriod.FourHours.Id)
            {
                if (timestamp.Hour % 4 != 0)
                {
                    throw new ExchangeAccessDomainException("The hour property of timestamp must be divisible by 4 on 4hr chart.");
                }
            }
            else if (period == CandlePeriod.OneDay.Id)
            {
            }
            else if (period == CandlePeriod.OneWeek.Id)
            {
                if (timestamp.DayOfWeek != DayOfWeek.Monday)
                {
                    throw new ExchangeAccessDomainException("The DayOfWeek property of timestamp must be equal to Monday.");
                }
            }
        }

        private int GetPeriodMinute()
        {
            var period = this._candlePeriodId;
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

        private int GetPeriodHour()
        {
            var period = this._candlePeriodId;
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

        private int GetPeriodDay()
        {
            var period = this._candlePeriodId;
            int days = 0;

            if (period == CandlePeriod.OneDay.Id)
            {
                days = 1;
            }

            return days;
        }

        private int GetPeriodWeek()
        {
            var period = this._candlePeriodId;
            int weeks = 0;

            if (period == CandlePeriod.OneWeek.Id)
            {
                weeks = 1;
            }

            return weeks;
        }
        #endregion
    }


}
