﻿using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.EventHandling
{
    public class LiveInvestmentStartedIntegrationEventHandler : IIntegrationEventHandler<LiveInvestmentStartedIntegrationEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public LiveInvestmentStartedIntegrationEventHandler(ITraceRepository traceRepository, ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(LiveInvestmentStartedIntegrationEvent @event)
        {
            try
            {
                var trace = await this._traceRepository.GetByTraceIdAsync(@event.TraceId);

                if (trace == null)
                {
                    return;
                }

                var timeService = new RealTimeService();

                trace.StartTracing(timeService);

                this._traceRepository.Update(trace);


                await _traceRepository.UnitOfWork
                    .SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: LiveInvestmentStartedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
