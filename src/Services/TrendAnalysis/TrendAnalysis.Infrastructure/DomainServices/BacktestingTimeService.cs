using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices
{
    public class BacktestingTimeService : ITimeService
    {
        private DateTime _currentDateTime;

        public BacktestingTimeService()
        {

        }

        public void SetCurrentDateTime(DateTime dateTime)
        {
            if (dateTime == null)
            {
                throw new ArgumentNullException(nameof(dateTime));
            }

            _currentDateTime = dateTime;
        }

        public DateTime GetCurrentDateTime()
        {
            return _currentDateTime;
        }
    }
}
