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
        public List<decimal?> Rsi(IEnumerable<Candle> source, int period = 14)
        {
            /*int outBegIdx, outNbElement;
            double[] rsiValues = new double[source.Count()];

            var closes = source.Select(x => Convert.ToDouble(x.Close)).ToArray();

            var ema = TicTacTec.TA.Library.Core.Rsi(0, source.Count() - 1, closes, period, out outBegIdx, out outNbElement, rsiValues);

            if (ema == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                return rsiValues.ToList().FixIndicatorOrdering(outBegIdx, outNbElement);
            }

            throw new Exception("Could not calculate RSI!");*/

            if (source.Count() < period + 1)
            {
                throw new Exception("Could not calculate RSI!");
            }

            var candles = source.OrderBy(c => c.Timestamp).ToList();
            var candleCount = candles.Count();

            var gains = new decimal[candleCount];
            var losses = new decimal[candleCount];
            var avgGains = new decimal?[candleCount];
            var avgLosses = new decimal?[candleCount];
            //var rs = new decimal[candleCount];
            var rsi = new decimal?[candleCount];

            #region Gains & Losses

            gains[0] = 0;
            losses[0] = 0;

            for (int i = 1; i < candleCount; i++)
            {
                decimal up = 0;
                decimal decline = 0;

                var previous = candles[i - 1];
                var current = candles[i];

                if (current.Close > previous.Close)
                {
                    up = current.Close - previous.Close;
                    decline = 0;
                }
                else if (current.Close < previous.Close)
                {
                    up = 0;
                    decline = previous.Close - current.Close;
                }
                else
                {
                    up = 0;
                    decline = 0;
                }

                gains[i] = up;
                losses[i] = decline;
            }

            #endregion


            #region Average gains & average losses

            decimal firstAvgGain = 0;
            decimal firstAvgLoss = 0;

            decimal sumOfGains = 0;
            decimal sumOfLosses = 0;

            for (int i = 1; i < period + 1; i++)
            {
                sumOfGains += gains[i];
                sumOfLosses += losses[i];
            }

            firstAvgGain = sumOfGains / period;
            firstAvgLoss = sumOfLosses / period;

            avgGains[period] = firstAvgGain;
            avgLosses[period] = firstAvgLoss;


            for (int i = period + 1; i < candleCount; i++)
            {
                var avgGain = (avgGains[i - 1] * (period - 1) + gains[i]) / period;
                var avgLoss = (avgLosses[i - 1] * (period - 1) + losses[i]) / period;

                avgGains[i] = avgGain;
                avgLosses[i] = avgLoss;
            }

            #endregion


            #region RSI

            for (int i = period; i < candleCount; i++)
            {
                rsi[i] = Math.Round(((decimal)avgGains[i] / ((decimal)avgGains[i] + (decimal)avgLosses[i])) * 100, 2);
            }

            #endregion

            return rsi.ToList();


            /*
            double[] rsiValues = new double[source.Count()];

            var closes = source.Select(x => Convert.ToDouble(x.Close)).ToArray();

            var rsi = new double[closes.Length];

            double gain = 0.0;
            double loss = 0.0;

            // first RSI value
            rsi[0] = 0.0;
            for (int i = 1; i <= period; ++i)
            {
                var diff = closes[i] - closes[i - 1];
                if (diff >= 0)
                {
                    gain += diff;
                }
                else
                {
                    loss -= diff;
                }
            }

            double avrg = gain / period;
            double avrl = loss / period;
            double rs = gain / loss;
            rsi[period] = 100 - (100 / (1 + rs));

            for (int i = period + 1; i < closes.Length; ++i)
            {
                var diff = closes[i] - closes[i - 1];

                if (diff >= 0)
                {
                    avrg = ((avrg * (period - 1)) + diff) / period;
                    avrl = (avrl * (period - 1)) / period;
                }
                else
                {
                    avrl = ((avrl * (period - 1)) - diff) / period;
                    avrg = (avrg * (period - 1)) / period;
                }

                rs = avrg / avrl;

                rsi[i] = 100 - (100 / (1 + rs));
            }

            var outValue = new List<decimal?>();
            foreach (var value in rsi)
            {
                outValue.Add((decimal?)value);
            }

            return outValue;
            */

        }
    }
}
