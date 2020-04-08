using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts
{
    public class Candle : Entity
    {
        public DateTime Timestamp { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }
        public decimal Open { get; private set; }
        public decimal Close { get; private set; }
        public decimal Volume { get; private set; }


        public Candle(DateTime timestamp, decimal high, decimal low, decimal open, decimal close, decimal volume)
        {
            Timestamp = timestamp;
            High = high;
            Low = low;
            Open = open;
            Close = close;
            Volume = volume;
        }

        public void UpdateData(decimal high, decimal low, decimal open, decimal close, decimal volume)
        {
            High = high;
            Low = low;
            Open = open;
            Close = close;
            Volume = volume;
        }

        /*protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Timestamp;
            yield return this.High;
            yield return this.Low;
            yield return this.Open;
            yield return this.Close;
            yield return this.Volume;
        }*/
    }
}
