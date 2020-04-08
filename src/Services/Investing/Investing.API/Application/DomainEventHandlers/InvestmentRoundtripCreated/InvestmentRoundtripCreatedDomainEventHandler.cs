using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentRoundtripCreated
{
    public class InvestmentRoundtripCreatedDomainEventHandler : INotificationHandler<InvestmentRoundtripCreatedDomainEvent>
    {
        private readonly IRoundtripRepository _roundtripRepository;

        public InvestmentRoundtripCreatedDomainEventHandler(IRoundtripRepository roundtripRepository)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
        }

        public async Task Handle(InvestmentRoundtripCreatedDomainEvent investmentRoundtripCreatedDomainEvent, CancellationToken cancellationToken)
        {
            try
            {
                var roundtrip = new Roundtrip(
                    investmentRoundtripCreatedDomainEvent.InvestmentId,
                    investmentRoundtripCreatedDomainEvent.RoundtripNumber,
                    investmentRoundtripCreatedDomainEvent.Market,
                    investmentRoundtripCreatedDomainEvent.EntryBalance,
                    investmentRoundtripCreatedDomainEvent.ExecutePrice,
                    investmentRoundtripCreatedDomainEvent.TargetPrice,
                    investmentRoundtripCreatedDomainEvent.StopLossPrice,
                    investmentRoundtripCreatedDomainEvent.DateCreated
                    );

                _roundtripRepository.Add(roundtrip);


                await _roundtripRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentRoundtripCreatedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
