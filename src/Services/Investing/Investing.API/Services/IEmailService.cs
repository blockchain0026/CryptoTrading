using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Services
{
    public interface IEmailService
    {
        Task Send(string title, string message, string fromMailAddress = null, string fromName = null, List<string> to = null);
    }
}
