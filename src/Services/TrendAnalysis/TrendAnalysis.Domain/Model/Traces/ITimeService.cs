using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces
{
    public interface ITimeService
    {
        DateTime GetCurrentDateTime();
    }
}
