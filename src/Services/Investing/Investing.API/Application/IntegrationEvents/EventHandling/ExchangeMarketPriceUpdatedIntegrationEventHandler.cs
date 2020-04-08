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
    public class ExchangeMarketPriceUpdatedIntegrationEventHandler : IIntegrationEventHandler<ExchangeMarketPriceUpdatedIntegrationEvent>
    {
        private readonly IRoundtripRepository _roundtripRepository;
        private readonly IInvestmentRepository _investmentRepository;

        public ExchangeMarketPriceUpdatedIntegrationEventHandler(IRoundtripRepository roundtripRepository, IInvestmentRepository investmentRepository)
        {
            _roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
        }

        public async Task Handle(ExchangeMarketPriceUpdatedIntegrationEvent @event)
        {
            //return;
            try
            {
                var runningRoundtrips = await _roundtripRepository.GetByStatus(RoundtripStatus.Entry);

                foreach (var roundtrip in runningRoundtrips)
                {
                    var investment = await _investmentRepository.GetByInvestmentId(roundtrip.InvestmentId);
                    if(investment.InvestmentType.Id==InvestmentType.Backtesting.Id)
                    {
                        continue;
                    }

                    if (roundtrip.Market.ExchangeId == @event.ExchangeId
                       && roundtrip.Market.BaseCurrency == @event.BaseCurrency
                       && roundtrip.Market.QuoteCurrency == @event.QuoteCurrency)
                    {
                        roundtrip.PriceChanged(@event.Price, @event.CreationDate);

                        this._roundtripRepository.Update(roundtrip);

                        try
                        {
                            await _roundtripRepository.UnitOfWork
                                .SaveEntitiesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Handle Integration Event: ExchangeMarketPriceUpdatedIntegrationEvent.");
                            Console.WriteLine("Result: Failure.");
                            Console.WriteLine("Error Message: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: ExchangeMarketPriceUpdatedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }

        }
    }
}
