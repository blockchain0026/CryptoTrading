using CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTacTec.TA.Library;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices
{
    public partial class IndicatorService : IIndicatorService
    {
        public Macd Macd(IEnumerable<Candle> candles, int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9)
        {
            if (candles.Count() < (slowPeriod + signalPeriod - 1))
            {
                throw new Exception("Could not calculate MACD!");
            }



            var candleData = candles.ToList().OrderBy(c => c.Timestamp).ToList();

            var candleCount = candles.Count();

            var diList = new decimal[candleCount];
            var emaFastList = new decimal?[candleCount];
            var emaSlowList = new decimal?[candleCount];
            var difList = new decimal?[candleCount];
            var macdList = new decimal?[candleCount];
            var oscList = new decimal?[candleCount];


            #region DI

            for (int i = 0; i < candleCount; i++)
            {
                var candle = candleData[i];

                var high = candle.High;
                var low = candle.Low;
                var open = candle.Open;
                var close = candle.Close;

                var di = (high + low + close * 2) / 4;
                diList[i] = di;
            }

            #endregion



            #region Ema Fast

            decimal firstEmaFast = 0;
            decimal emaFastWarmSum = 0;

            for (int i = 0; i < fastPeriod; i++)
            {
                emaFastWarmSum += diList[i];
            }

            firstEmaFast = emaFastWarmSum / fastPeriod;
            emaFastList[fastPeriod - 1] = firstEmaFast;

            for (int i = fastPeriod; i < candleCount; i++)
            {
                var previous = emaFastList[i - 1];
                var emaFast = (previous * (fastPeriod - 1) + diList[i] * 2) / (fastPeriod + 1);
                emaFastList[i] = emaFast;
            }

            #endregion


            #region Ema Slow

            decimal firstEmaSlow = 0;
            decimal emaSlowWarmSum = 0;

            for (int i = 0; i < slowPeriod; i++)
            {
                emaSlowWarmSum += diList[i];
            }

            firstEmaSlow = emaSlowWarmSum / slowPeriod;
            emaSlowList[slowPeriod - 1] = firstEmaSlow;

            for (int i = slowPeriod; i < candleCount; i++)
            {
                var previous = emaSlowList[i - 1];
                var emaSlow = (previous * (slowPeriod - 1) + diList[i] * 2) / (slowPeriod + 1);
                emaSlowList[i] = emaSlow;
            }

            #endregion


            #region DIF

            for (int i = slowPeriod - 1; i < candleCount; i++)
            {
                var dif = emaFastList[i] - emaSlowList[i];
                difList[i] = dif;
            }

            #endregion


            #region Macd

            decimal firstMacd = 0;
            decimal difWarmSum = 0;

            for (int i = slowPeriod - 1; i < slowPeriod - 1 + signalPeriod; i++)
            {
                difWarmSum += (decimal)difList[i];
            }

            firstMacd = difWarmSum / signalPeriod;
            macdList[slowPeriod - 1 + signalPeriod - 1] = firstMacd;

            for (int i = slowPeriod - 1 + signalPeriod; i < candleCount; i++)
            {
                var previous = macdList[i - 1];
                var macd = (previous * (signalPeriod - 1) + (decimal)difList[i] * 2) / (signalPeriod + 1);

                macdList[i] = macd;
            }

            #endregion


            #region

            for (int i = slowPeriod - 1 + signalPeriod - 1; i < candleCount; i++)
            {
                oscList[i] = difList[i] - macdList[i];
            }

            #endregion


         

            var macdItem = new Macd
            {
                DIF = difList.ToList(),
                DEM = macdList.ToList(),
                OSC = oscList.ToList()
            };


            return macdItem;

            /*int outBegIdx, outNbElement;
            double[] macdValues = new double[candles.Count()];
            double[] signalValues = new double[candles.Count()];
            double[] histValues = new double[candles.Count()];
            var closes = candles.Select(x => Convert.ToDouble(x.Close)).ToArray();

            var macd = TicTacTec.TA.Library.Core.Macd(0, candles.Count() - 1, closes,
                fastPeriod, slowPeriod, signalPeriod, out outBegIdx, out outNbElement, macdValues, signalValues, histValues);

            if (macd == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                return new Macd()
                {
                    Value = macdValues.ToList().FixIndicatorOrdering(outBegIdx, outNbElement),
                    Signal = signalValues.ToList().FixIndicatorOrdering(outBegIdx, outNbElement),
                    Hist = histValues.ToList().FixIndicatorOrdering(outBegIdx, outNbElement)
                };
            }

            throw new Exception("Could not calculate MACD!"); */
        }
    }
}
