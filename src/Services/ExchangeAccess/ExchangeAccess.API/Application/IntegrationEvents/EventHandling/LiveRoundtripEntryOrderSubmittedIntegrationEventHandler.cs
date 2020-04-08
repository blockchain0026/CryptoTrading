using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.EventHandling
{
    public class LiveRoundtripEntryOrderSubmittedIntegrationEventHandler : IIntegrationEventHandler<LiveRoundtripEntryOrderSubmittedIntegrationEvent>
    {
        private readonly IExchangeAccessIntegrationEventService _exchangeAccessIntegrationEventService;
        private readonly IMarketRepository _marketRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IExchangeAccessService _exchangeAccessService;

        public LiveRoundtripEntryOrderSubmittedIntegrationEventHandler(IExchangeAccessIntegrationEventService exchangeAccessIntegrationEventService, IMarketRepository marketRepository, IBalanceRepository balanceRepository, IExchangeAccessService exchangeAccessService)
        {
            _exchangeAccessIntegrationEventService = exchangeAccessIntegrationEventService ?? throw new ArgumentNullException(nameof(exchangeAccessIntegrationEventService));
            _marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _exchangeAccessService = exchangeAccessService ?? throw new ArgumentNullException(nameof(exchangeAccessService));
        }

        public async Task Handle(LiveRoundtripEntryOrderSubmittedIntegrationEvent @event)
        {
            try
            {
                var market = await this._marketRepository.GetByCurrencyPairAsync(@event.BaseCurrency, @event.QuoteCurrency, @event.ExchangeId);
                var balance = await this._balanceRepository.GetByUsernameAsync(@event.Username);

                if (market == null || balance == null)
                {
                    return;
                }

                var buyAmount = @event.EntryAmount / @event.EntryPrice;
                var orderId = market.CreateBuyOrder(
                    balance,
                    @event.BaseCurrency,
                    @event.QuoteCurrency,
                    @event.EntryPrice,
                    buyAmount,
                    this._exchangeAccessService
                    );


                if (orderId != null)
                {

                    var order = await _exchangeAccessService.GetOrder(
                        balance.Account,
                        @event.ExchangeId,
                        orderId,
                        @event.BaseCurrency,
                        @event.QuoteCurrency
                        );

                    await this._exchangeAccessIntegrationEventService.PublishThroughEventBusAsync(new LiveRoundtripEntryOrderExecutedIntegrationEvent(
                        @event.InvestmentId,
                        @event.RoundtripId,
                        @event.ExchangeId,
                        orderId,
                        @event.BaseCurrency,
                        @event.QuoteCurrency,
                        order.ExecutedQuantity,
                        order.Price,
                        order.CommisionPaid
                        ));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integraion Event: LiveRoundtripEntryOrderSubmittedIntegrationEvent \n" +
      
                    "Result: Failure. \n" +
    
                    "Error Message: " + ex.Message);
            }
        }
    }
}
