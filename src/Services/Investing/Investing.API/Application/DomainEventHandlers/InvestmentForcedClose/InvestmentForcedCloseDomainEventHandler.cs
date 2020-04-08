using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentForcedClose
{
    public class InvestmentForcedCloseDomainEventHandler : INotificationHandler<InvestmentForcedCloseDomainEvent>
    {

        private readonly IRoundtripRepository _roundtripRepository;

        public InvestmentForcedCloseDomainEventHandler(IRoundtripRepository roundtripRepository)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
        }

        public async Task Handle(InvestmentForcedCloseDomainEvent notification, CancellationToken cancellationToken)
        {
            var investment = notification.Investment;
            var roundtrips = await _roundtripRepository.GetByInvestmentId(investment.InvestmentId);
            if (roundtrips.Any())
            {
                foreach (var roundtrip in roundtrips)
                {
                    if (roundtrip.GetStatus().Id != RoundtripStatus.Exit.Id && roundtrip.GetStatus().Id != RoundtripStatus.ForceExit.Id)
                    {
                        roundtrip.ForceSelling();
                    }
                    _roundtripRepository.Update(roundtrip);
                }
            }

            try
            {
                await this._roundtripRepository.UnitOfWork.SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentForcedCloseDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
