using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentSellingAdviceCreated
{

    public class InvestmentSellingAdviceCreatedDomainEventHandler : INotificationHandler<InvestmentSellingAdviceCreatedDomainEvent>
    {
        private readonly IRoundtripRepository _roundtripRepository;

        public InvestmentSellingAdviceCreatedDomainEventHandler(IRoundtripRepository roundtripRepository)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
        }

        public async Task Handle(InvestmentSellingAdviceCreatedDomainEvent investmentSellingAdviceCreatedDomainEvent, CancellationToken cancellationToken)
        {


            var roundtrips = await this._roundtripRepository.GetByInvestmentId(investmentSellingAdviceCreatedDomainEvent.InvestmentId);


            try
            {
                if (roundtrips.Any())
                {
                    var roundtrip = roundtrips.Where(r => r.RoundtripNumber == investmentSellingAdviceCreatedDomainEvent.RoundtripNumber).SingleOrDefault();
                    if (roundtrip == null)
                    {
                        return;
                    }
                    if (roundtrip.GetStatus().Id != RoundtripStatus.Entry.Id)
                    {
                        return;
                    }

                    roundtrip.Exit(
                        investmentSellingAdviceCreatedDomainEvent.ExecutePrice,
                        investmentSellingAdviceCreatedDomainEvent.DateCreated);

                    this._roundtripRepository.Update(roundtrip);

                    await _roundtripRepository.UnitOfWork
                        .SaveEntitiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentSellingAdviceCreatedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
