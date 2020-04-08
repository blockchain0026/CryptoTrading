using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators
{
    public partial interface IIndicatorService
    {
        Macd Macd(IEnumerable<Candle> candles, int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9);
    }

    public class Macd
    {
        public List<decimal?> DIF { get; set; }
        public List<decimal?> DEM { get; set; }
        public List<decimal?> OSC { get; set; }
    }

  
}
