using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.Idempotency
{
    public class ClientRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}
