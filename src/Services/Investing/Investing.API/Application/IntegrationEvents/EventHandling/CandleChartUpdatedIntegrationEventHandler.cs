using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{

    public class CandleChartUpdatedIntegrationEventHandler : IIntegrationEventHandler<CandleChartUpdatedIntegrationEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public CandleChartUpdatedIntegrationEventHandler(IInvestmentRepository investmentRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(CandleChartUpdatedIntegrationEvent @event)
        {
            try
            {


                var runningInvestment = await this._investmentRepository.GetByStatus(InvestmentStatus.Started);

                foreach (var investment in runningInvestment)
                {
                    var market = new Market(@event.ExchangeId, @event.BaseCurrency.ToUpper(), @event.QuoteCurrency.ToUpper());
                    if (investment.Market.ExchangeId == @event.ExchangeId
                        && investment.Market.BaseCurrency == @event.BaseCurrency
                        && investment.Market.QuoteCurrency == @event.QuoteCurrency)
                    {

                        if (investment.InvestmentType.Id != InvestmentType.Backtesting.Id)
                        {
                            await this._investingIntegrationEventService.PublishThroughEventBusAsync(new InvestmentCandleDataRequestedIntegrationEvent(
                                @event.CandleChartId,
                                @event.ExchangeId,
                                @event.BaseCurrency,
                                @event.QuoteCurrency,
                                @event.CandlePeriod,
                                @event.Timestamp,
                                @event.High,
                                @event.Low,
                                @event.Open,
                                @event.Close,
                                @event.Volume
                                ));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: CandleChartUpdatedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
