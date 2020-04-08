using CryptoTrading.Services.TrendAnalysis.Domain.Exceptions;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies
{
    public class Strategy : ValueObject
    {
        public string Name { get; private set; }
        public int MinimumAmountOfCandles { get; private set; }
        public CandlePeriod CandlePeriod { get; private set; }
        private int _candlePeriodId;

        public Strategy(string name, int minimumAmountOfCandles, int candlePeriodId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MinimumAmountOfCandles = minimumAmountOfCandles;
            //CandlePeriod = CandlePeriod.From(candlePeriodId) ?? throw new ArgumentOutOfRangeException(nameof(candlePeriodId));
            _candlePeriodId = CandlePeriod.From(candlePeriodId)!=null?candlePeriodId: throw new ArgumentOutOfRangeException(nameof(candlePeriodId));
        }


        public DateTime GetCurrentCandleDateTime(ITimeService timeService)
        {
            if (timeService == null)
            {
                throw new ArgumentNullException();
            }

            var dateTime = timeService.GetCurrentDateTime();

            var minute = this.GetPeriodMinute();
            var hour = this.GetPeriodHour();
            var day = this.GetPeriodDay();
            var week = this.GetPeriodWeek();

            if (minute == 0 && hour == 0 && day == 0)
            {
                DateTime monday = dateTime;
                while (monday.DayOfWeek != DayOfWeek.Monday)
                {
                    monday = monday.AddDays(-1);
                }

                return new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, 0);
            }
            else if (minute == 0 && hour == 0)
            {
                if (dateTime.Hour != 0 || dateTime.Minute != 0 || dateTime.Second != 0 || dateTime.Millisecond != 0)
                {

                    var substractDays = dateTime.AddDays(-1);
                    return new DateTime(substractDays.Year, substractDays.Month, substractDays.Day, 0, 0, 0, 0);
                }
                else
                {
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);

                }
            }
            else if (minute == 0)
            {
                if (dateTime.Hour % hour != 0
                    || dateTime.Minute != 0 || dateTime.Second != 0 || dateTime.Millisecond != 0)
                {
                    int hoursToSub = dateTime.Hour % hour;
                    var subHours = dateTime.AddHours(-hoursToSub);
                    return new DateTime(subHours.Year, subHours.Month, subHours.Day, subHours.Hour, 0, 0, 0);
                }
                else
                {
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0);
                }
            }
            else
            {
                if (dateTime.Minute % minute != 0
                    || dateTime.Second != 0 || dateTime.Millisecond != 0)
                {

                    int minutesToSub = dateTime.Minute % minute;
                    var subMinutes = dateTime.AddMinutes(-minutesToSub);
                    return new DateTime(subMinutes.Year, subMinutes.Month, subMinutes.Day, subMinutes.Hour, subMinutes.Minute, 0, 0);
                }
                else
                {
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0);
                }
            }
        }

        public virtual TradingSignalType Analysis(IEnumerable<Candle> candles, IIndicatorService indicatorService)
        {
            var strategies = GetAllStrategies();
            foreach(var strategy in strategies)
            {
                if(strategy.Name == this.Name)
                {
                    return strategy.Analysis(candles, indicatorService);
                }
            }
            throw new TrendAnalysisDomainException($"There is no strategy called {this.Name}.");
        }

        public static Strategy GetStrategy(string name)
        {
            var strategies = GetAllStrategies();

            foreach (var strategy in strategies)
            {
                if (strategy.Name.ToUpper() == name.ToUpper())
                {
                    return strategy;
                }
            }

            return null;
        }

        public static IEnumerable<Strategy> GetAllStrategies()
        {
            var strategies = new List<Strategy>();

            strategies.Add(new AwesomeMacd());
            strategies.Add(new RsiMacd());

            return strategies;
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



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Name;
            yield return this.MinimumAmountOfCandles;
            yield return this.CandlePeriod.Id;
        }
    }
}
