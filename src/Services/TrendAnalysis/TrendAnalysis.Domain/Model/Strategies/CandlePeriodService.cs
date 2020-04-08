using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies
{
    public static class CandlePeriodService
    {
        public static int GetOneCandleMinutesByPeriod(CandlePeriod candlePeriod)
        {
            var period = candlePeriod.Id;

            if (period == CandlePeriod.OneMinute.Id)
            {
                return 1;
            }
            else if (period == CandlePeriod.FiveMinutes.Id)
            {
                return 5;
            }
            else if (period == CandlePeriod.FifteenMinutes.Id)
            {
                return 15;
            }
            else if (period == CandlePeriod.ThirtyMinutes.Id)
            {
                return 30;
            }
            else if (period == CandlePeriod.OneHour.Id)
            {
                return 60;
            }
            else if (period == CandlePeriod.TwoHours.Id)
            {
                return 60 * 2;
            }
            else if (period == CandlePeriod.FourHours.Id)
            {
                return 60 * 4;
            }
            else if (period == CandlePeriod.OneDay.Id)
            {
                return 60 * 24;
            }
            else if (period == CandlePeriod.OneWeek.Id)
            {
                return 60 * 24 * 7;
            }

            throw new ArgumentOutOfRangeException("Could not recognize this candle period.");
        }
    }
}
