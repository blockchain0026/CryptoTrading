using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.Commands;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.EventHandling
{
    public class InvestmentSettledIntegrationEventHandler : IIntegrationEventHandler<InvestmentSettledIntegrationEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly IMediator _mediator;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;

        public InvestmentSettledIntegrationEventHandler(ITraceRepository traceRepository, IMediator mediator, ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(InvestmentSettledIntegrationEvent @event)
        {
            var request = new CreateTraceCommand(
                @event.InvestmentId,
                @event.ExchangeId,
                @event.BaseCurrency,
                @event.QuoteCurrency,
                new string[] { }
                );

            await this._mediator.Send(request);
        }
    }
}
