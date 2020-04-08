using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this long unixTimestamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            return dtDateTime.AddSeconds(unixTimestamp);
        }

        public static long ToTimestamp(this DateTime dateTime)
        {
            return (int)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
