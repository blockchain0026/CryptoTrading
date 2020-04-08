using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators
{

    public partial interface IIndicatorService
    {
        List<decimal?> Sma(IEnumerable<decimal> source, int period = 30);
    }
}
