using CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices
{
    public partial class IndicatorService : IIndicatorService
    {
        public List<decimal?> Sma(IEnumerable<decimal> source, int period = 30)
        {
            int outBegIdx, outNbElement;
            double[] smaValues = new double[source.Count()];
            List<double?> outValues = new List<double?>();

            var sourceFix = source.Select(x => Convert.ToDouble(x)).ToArray();

            var sma = TicTacTec.TA.Library.Core.Sma(0, source.Count() - 1, sourceFix, period, out outBegIdx, out outNbElement, smaValues);

            if (sma == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                return smaValues.ToList().FixIndicatorOrdering(outBegIdx, outNbElement);
            }

            throw new Exception("Could not calculate SMA!");
        }
    }
}
