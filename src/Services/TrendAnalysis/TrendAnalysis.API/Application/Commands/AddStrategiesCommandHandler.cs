using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.Idempotency;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.Commands
{
    public class AddStrategiesCommandHandler : IRequestHandler<AddStrategiesCommand, bool>
    {
        private readonly ITraceRepository _traceRepository;
        //private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus;

        public AddStrategiesCommandHandler(ITraceRepository traceRepository, IMediator mediator, IEventBus eventBus)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> Handle(AddStrategiesCommand request, CancellationToken cancellationToken)
        {
            //Add strategies.
            var createdTrace = await _traceRepository.GetByTraceIdAsync(request.TraceId);


            var strategies = new List<Strategy>();

            foreach (var strategyName in request.Strategies)
            {
                var strategy = Strategy.GetStrategy(strategyName);

                if (strategy == null)
                {
                    throw new KeyNotFoundException($"Strategy not found: {strategyName}");
                }

                strategies.Add(strategy);
            }


            var timeService = new BacktestingTimeService();
            timeService.SetCurrentDateTime(DateTime.UtcNow);

            foreach (var strategy in strategies)
            {
                createdTrace.AddStrategy(strategy, timeService);
            }


            _traceRepository.Update(createdTrace);


            try
            {
                await _traceRepository.UnitOfWork
                    .SaveEntitiesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Adding strategies to trace failed. Error occured when adding strategies to a trace. \n" +
                    "Error Message: " + ex.Message);
            }

            return false;
        }
    }

    // Use for Idempotency in Command process
    public class AddStrategiesIdentifiedCommandHandler : IdentifiedCommandHandler<AddStrategiesCommand, bool>
    {
        public AddStrategiesIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;        // Ignore duplicate requests for creating order.
        }
    }
}
