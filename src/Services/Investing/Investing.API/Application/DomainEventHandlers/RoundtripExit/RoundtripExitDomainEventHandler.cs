using CryptoTrading.Services.Investing.API.Application.IntegrationEvents;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.RoundtripExit
{
    public class RoundtripExitDomainEventHandler : INotificationHandler<RoundtripExitDomainEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public RoundtripExitDomainEventHandler(IInvestmentRepository investmentRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(RoundtripExitDomainEvent roundtripExitDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                //Already has integration event to handle this.

                /*var roundtrip = roundtripExitDomainEvent.Roundtrip;

                var investment = await this._investmentRepository.GetByInvestmentId(roundtrip.InvestmentId);

                if (investment == null)
                {
                    throw new KeyNotFoundException(roundtrip.InvestmentId);
                }

                investment.RoundtripExit(roundtrip);

                this._investmentRepository.Update(investment);


                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: RoundtripExitDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
