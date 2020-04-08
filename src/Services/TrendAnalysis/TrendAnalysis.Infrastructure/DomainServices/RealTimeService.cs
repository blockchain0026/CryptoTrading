using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices
{
    public class RealTimeService : ITimeService
    {
        public DateTime GetCurrentDateTime()
        {
            var utcNow = DateTime.UtcNow;
            return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second);
        }
    }
}
