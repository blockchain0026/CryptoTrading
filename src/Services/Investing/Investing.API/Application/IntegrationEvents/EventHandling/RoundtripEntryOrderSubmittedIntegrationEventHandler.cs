using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class RoundtripEntryOrderSubmittedIntegrationEventHandler : IIntegrationEventHandler<RoundtripEntryOrderSubmittedIntegrationEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public RoundtripEntryOrderSubmittedIntegrationEventHandler(IInvestmentRepository investmentRepository, IRoundtripRepository roundtripRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(RoundtripEntryOrderSubmittedIntegrationEvent @event)
        {
            try
            {
                var investment = await this._investmentRepository.GetByInvestmentId(@event.InvestmentId);

                if (investment == null)
                {
                    throw new KeyNotFoundException(nameof(@event.InvestmentId));
                }


                if (investment.InvestmentType.Id == InvestmentType.Live.Id)
                {
                    var slipEntryPrice = @event.EntryPrice * 1.01M;
                    //The min notation the exchange accept.
                    var entryAmount = @event.EntryAmount.ToString("N6");
                    await this._investingIntegrationEventService
                        .PublishThroughEventBusAsync(new LiveRoundtripEntryOrderSubmittedIntegrationEvent(
                            investment.InvestmentId,
                            investment.Account.Username,
                            @event.RoundtripId,
                            @event.ExchangeId,
                            @event.BaseCurrency,
                            @event.QuoteCurrency,
                            Decimal.Parse(entryAmount, System.Globalization.NumberStyles.Float),
                            slipEntryPrice
                            ));
                }
                else
                {
                    var roundtrip = await this._roundtripRepository.GetByRoundtripId(@event.RoundtripId);

                    if (roundtrip == null)
                    {
                        throw new KeyNotFoundException(nameof(@event.RoundtripId));
                    }

                    roundtrip.OrderFilled(
                        @event.EntryAmount / @event.EntryPrice,
                        @event.EntryPrice,
                        @event.AdviceCreationDate);

                    this._roundtripRepository.Update(roundtrip);

                    if (investment.InvestmentType.Id == InvestmentType.Paper.Id)
                    {
                        await _roundtripRepository.UnitOfWork
                            .SaveEntitiesAsync();
                    }
                    else
                    {
                        await _roundtripRepository.UnitOfWork
                            .SaveChangesAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: RoundtripEntryOrderSubmittedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
