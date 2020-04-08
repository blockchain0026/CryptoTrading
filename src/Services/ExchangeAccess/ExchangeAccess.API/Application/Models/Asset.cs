using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.Models
{
    public class Asset
    {
        public string Symbol { get; set; }
        public decimal Total { get; set; }
        public decimal Available { get; set; }
        public decimal Locked { get; set; }
    }
}
