using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators
{
    public static partial class Indicators
    {
        public static int TrendDetermine(this IEnumerable<Candle> source, int period = 14)
        {
            //-1=downtrend,0=sideways,1=uptrend
            throw new NotImplementedException();
        }
    }

}
