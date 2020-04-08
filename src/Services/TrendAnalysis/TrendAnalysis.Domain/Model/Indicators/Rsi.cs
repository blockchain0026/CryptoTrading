using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators
{
    public partial interface IIndicatorService
    {
        List<decimal?> Rsi(IEnumerable<Candle> source, int period = 14);
    }
}
