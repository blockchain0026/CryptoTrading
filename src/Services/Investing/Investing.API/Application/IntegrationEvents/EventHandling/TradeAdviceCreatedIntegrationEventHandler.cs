using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class TradeAdviceCreatedIntegrationEventHandler : IIntegrationEventHandler<TradeAdviceCreatedIntegrationEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IInvestingIntegrationEventService _investingIntegrationEventService;

        public TradeAdviceCreatedIntegrationEventHandler(IInvestmentRepository investmentRepository, IInvestingIntegrationEventService investingIntegrationEventService)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _investingIntegrationEventService = investingIntegrationEventService ?? throw new ArgumentNullException(nameof(investingIntegrationEventService));
        }

        public async Task Handle(TradeAdviceCreatedIntegrationEvent @event)
        {
            try
            {
                var investment = await this._investmentRepository.GetByTrace(new Trace(@event.TraceId));
                if (investment == null)
                {
                    return;
                }

                if (@event.TradingSignalType.ToUpper() == "BUY")
                {
                    investment.BuyingAdvice(@event.TraceId, @event.Price, @event.TargetPrice, @event.DateCreated);
                    this._investmentRepository.Update(investment);
                }
                else if (@event.TradingSignalType.ToUpper() == "SELL")
                {
                    investment.SellingAdvice(@event.TraceId, @event.Price, @event.DateCreated);
                    this._investmentRepository.Update(investment);
                }
                else
                {
                    return;
                }



                await _investmentRepository.UnitOfWork
                    .SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: TradeAdviceCreatedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
