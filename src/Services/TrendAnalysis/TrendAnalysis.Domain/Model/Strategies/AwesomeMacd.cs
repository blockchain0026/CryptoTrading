using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies
{
    public class AwesomeMacd : Strategy
    {
        public AwesomeMacd(string name = "Awesome MACD", int minimumAmountOfCandles = 40, int candlePeriodId = 3) 
            : base(name, minimumAmountOfCandles, candlePeriodId)
        {
        }

        public override TradingSignalType Analysis(IEnumerable<Candle> candles, IIndicatorService indicatorService)
        {
            var result = new List<TradingSignalType>();

            var ao = indicatorService.AwesomeOscillator(candles);
            var macd = indicatorService.Macd(candles,5, 7, 4);
            
            for (int i = 0; i < candles.Count(); i++)
            {
                if (i == 0)
                    result.Add(TradingSignalType.Hold);

                else if (ao[i] < 0 && ao[i - 1] > 0 && macd.OSC[i] < 0)
                    result.Add(TradingSignalType.Sell);

                else if (ao[i] > 0 && ao[i - 1] < 0 && macd.OSC[i] > 0)
                    result.Add(TradingSignalType.Buy);

                else
                    result.Add(TradingSignalType.Hold);
            }

            return result.Last();
        }
    }
}
