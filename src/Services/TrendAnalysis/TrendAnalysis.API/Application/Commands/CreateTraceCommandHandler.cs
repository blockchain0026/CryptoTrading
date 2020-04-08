using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.Idempotency;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.Commands
{
    // Regular CommandHandler
    public class CreateTraceCommandHandler
        : IRequestHandler<CreateTraceCommand, bool>
    {
        private readonly ITraceRepository _traceRepository;
        //private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus;

        // Using DI to inject infrastructure persistence Repositories
        public CreateTraceCommandHandler(IMediator mediator, ITraceRepository traceRepository, IEventBus eventBus)
        {
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            //_identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> Handle(CreateTraceCommand message, CancellationToken cancellationToken)
        {
            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate


            //Create a new trace.
            var trace = new Trace(
                message.InvestmentId,
                message.ExchangeId,
                message.BaseCurrency,
                message.QuoteCurrency
                );


            _traceRepository.Add(trace);

            try
            {
                await _traceRepository.UnitOfWork
                    .SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Create trace failed. \n" +
                    "Error Message: " + ex.Message);

                return false;
            }


          



            this._eventBus.Publish(new TraceCreatedIntegrationEvent(
                trace.TraceId,
                trace.Investment.InvestmentId,
                trace.Market.ExchangeId,
                trace.Market.BaseCurrency,
                trace.Market.QuoteCurrency
                ));

            return true;
        }
    }


    // Use for Idempotency in Command process
    public class CreateTraceIdentifiedCommandHandler : IdentifiedCommandHandler<CreateTraceCommand, bool>
    {
        public CreateTraceIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;        // Ignore duplicate requests for creating order.
        }
    }
}
