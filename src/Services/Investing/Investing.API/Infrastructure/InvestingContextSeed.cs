using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using CryptoTrading.Services.Investing.Infrastructure;
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

namespace CryptoTrading.Services.Investing.API.Infrastructure
{
    public class InvestingContextSeed
    {
        public static async Task SimpleSeedAsync(IApplicationBuilder applicationBuilder)
        {

            using (var context = (InvestingContext)applicationBuilder
             .ApplicationServices.GetService(typeof(InvestingContext)))
            {
                /*var connection = context.Database.GetDbConnection();
                var isConnected = connection.State==System.Data.ConnectionState.Open;*/


                context.Database.Migrate();
                /*connection.Open();
                isConnected = connection.State == System.Data.ConnectionState.Open;*/


                if (!context.InvestmentStatus.Any())
                {
                    context.InvestmentStatus.AddRange(GetPredefinedInvestmentStatus());
                }

                if (!context.InvestmentType.Any())
                {
                    context.InvestmentType.AddRange(GetPredefinedInvestmentType());
                }

                if (!context.RoundtripStatus.Any())
                {
                    context.RoundtripStatus.AddRange(GetPredefinedRoundtripStatus());
                }

                if (!context.InvestingFundStatus.Any())
                {
                    context.InvestingFundStatus.AddRange(GetPredefinedInvestingFundStatus());
                }

                await context.SaveChangesAsync();
            }
        }


        #region Predefined

        private static IEnumerable<InvestmentStatus> GetPredefinedInvestmentStatus()
        {
            return new List<InvestmentStatus>()
            {
                InvestmentStatus.Prepare,
                InvestmentStatus.Settled,
                InvestmentStatus.Ready,
                InvestmentStatus.Started,
                InvestmentStatus.ForcedClosing,
                InvestmentStatus.Closed,
            };
        }

        private static IEnumerable<InvestmentType> GetPredefinedInvestmentType()
        {
            return new List<InvestmentType>()
            {
                InvestmentType.Live,
                InvestmentType.Paper,
                InvestmentType.Backtesting
            };
        }

        private static IEnumerable<RoundtripStatus> GetPredefinedRoundtripStatus()
        {
            return new List<RoundtripStatus>()
            {
                RoundtripStatus.EntryOrderSubmitted,
                RoundtripStatus.Entry,
                RoundtripStatus.ExitOrderSubmitted,
                RoundtripStatus.Exit,
                RoundtripStatus.ForceSell,
                RoundtripStatus.ForceExit,
            };
        }


        private static IEnumerable<InvestingFundStatus> GetPredefinedInvestingFundStatus()
        {
            return new List<InvestingFundStatus>()
            {
                InvestingFundStatus.Initialize,
                InvestingFundStatus.Ended
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


        private Policy CreatePolicy(ILogger<InvestingContextSeed> logger, string prefix, int retries = 3)
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
