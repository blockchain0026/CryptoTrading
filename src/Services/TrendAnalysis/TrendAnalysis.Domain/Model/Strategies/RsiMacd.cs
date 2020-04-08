using CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies
{
    public class RsiMacd : Strategy
    {
        public RsiMacd(string name = "RSI MACD", int minimumAmountOfCandles = 350, int candlePeriodId = 3)
            : base(name, minimumAmountOfCandles, candlePeriodId)
        {
        }

        public override TradingSignalType Analysis(IEnumerable<Candle> candles, IIndicatorService indicatorService)
        {
            var result = new List<TradingSignalType>();
            var lastCandle = candles.Last();
            var _macd = indicatorService.Macd(candles, 12, 26, 9);

            var difLast = _macd.DIF.Last();
            var demLast = _macd.DEM.Last();
            var oscLast = _macd.OSC.Last();


            var _rsi = indicatorService.Rsi(candles, 14);

            var last = _rsi.Last();



            if (/*(decimal)oscLast > 0 &&*/ last < 40)
            {
                return TradingSignalType.Buy;
            }
            else if (/*(decimal)oscLast < 0 &&*/ last > 65)
            {
                return TradingSignalType.Sell;
            }
            else
            {
                return TradingSignalType.Hold;
            }





            /*for (int i = 0; i < candles.Count(); i++)
            {
                if (_rsi[i] > 70 && (_macd.Value[i] - _macd.Signal[i]) < 0)
                    result.Add(TradingSignalType.Sell);
                else if (_rsi[i] < 30 && (_macd.Value[i] - _macd.Signal[i]) > 0)
                    result.Add(TradingSignalType.Buy);
                else
                    result.Add(TradingSignalType.Hold);
            }

            return result.Last();*/
        }
    }
}
