using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Funds
{
    public interface IBalanceService
    {
        IEnumerable<Balance> GetAccountBalances(Account account, string symbol = null);
    }
}
