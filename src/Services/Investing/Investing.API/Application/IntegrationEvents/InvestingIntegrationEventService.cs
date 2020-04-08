using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.BuildingBlocks.EventBus.Events;
using CryptoTrading.BuildingBlocks.EventBus.IntegrationEventLogEF.Services;
using CryptoTrading.BuildingBlocks.EventBus.IntegrationEventLogEF.Utilities;
using CryptoTrading.Services.Investing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents
{

    public class InvestingIntegrationEventService : IInvestingIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly InvestingContext _investingContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public InvestingIntegrationEventService(IEventBus eventBus, InvestingContext investingContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _investingContext = investingContext ?? throw new ArgumentNullException(nameof(investingContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_investingContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            await SaveEventAndOrderingContextChangesAsync(evt);
            _eventBus.Publish(evt);
            //await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        private async Task SaveEventAndOrderingContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_investingContext)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original ordering database operation and the IntegrationEventLog thanks to a local transaction
                    await _investingContext.SaveChangesAsync();
                    //await _eventLogService.SaveEventAsync(evt, _executionContext.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }
}
