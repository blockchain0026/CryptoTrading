﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.Idempotency
{
    public class RequestManager : IRequestManager
    {
        private readonly TrendAnalysisContext _context;

        public RequestManager(TrendAnalysisContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.
                FindAsync<ClientRequest>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await ExistAsync(id);

            var request = exists ?
                throw new Exception($"Request with {id} already exists") :
                new ClientRequest()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);

            await _context.SaveChangesAsync();
        }
    }
}
