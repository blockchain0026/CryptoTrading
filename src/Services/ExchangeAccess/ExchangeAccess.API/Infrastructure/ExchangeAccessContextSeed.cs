using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Infrastructure
{
    public class ExchangeAccessContextSeed
    {
        public static async Task SimpleSeedAsync(IApplicationBuilder applicationBuilder)
        {

            using (var context = (ExchangeAccessContext)applicationBuilder
             .ApplicationServices.GetService(typeof(ExchangeAccessContext)))
            {
                /*var connection = context.Database.GetDbConnection();
                var isConnected = connection.State==System.Data.ConnectionState.Open;*/


                context.Database.Migrate();
                /*connection.Open();
                isConnected = connection.State == System.Data.ConnectionState.Open;*/


                if (!context.CandlePeriod.Any())
                {
                    context.CandlePeriod.AddRange(GetPredefinedCandlePeriod());
                }

             
                await context.SaveChangesAsync();
            }
        }


        #region CandlePeriod

        private static IEnumerable<CandlePeriod> GetPredefinedCandlePeriod()
        {
            return new List<CandlePeriod>()
            {
                CandlePeriod.OneMinute,
                CandlePeriod.FiveMinutes,
                CandlePeriod.FifteenMinutes,
                CandlePeriod.ThirtyMinutes,
                CandlePeriod.OneHour,
                CandlePeriod.TwoHours,
                CandlePeriod.FourHours,
                CandlePeriod.OneDay,
                CandlePeriod.OneWeek
            };
        }

        #endregion



        private string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }


        private Policy CreatePolicy(ILogger<ExchangeAccessContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
