﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.Idempotency
{
    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);

        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}
