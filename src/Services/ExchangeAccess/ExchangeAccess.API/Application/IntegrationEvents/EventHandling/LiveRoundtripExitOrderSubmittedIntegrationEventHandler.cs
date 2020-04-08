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
    public class LiveRoundtripExitOrderSubmittedIntegrationEventHandler : IIntegrationEventHandler<LiveRoundtripExitOrderSubmittedIntegrationEvent>
    {
        private readonly IExchangeAccessIntegrationEventService _exchangeAccessIntegrationEventService;
        private readonly IMarketRepository _marketRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IExchangeAccessService _exchangeAccessService;

        public LiveRoundtripExitOrderSubmittedIntegrationEventHandler(IExchangeAccessIntegrationEventService exchangeAccessIntegrationEventService, IMarketRepository marketRepository, IBalanceRepository balanceRepository, IExchangeAccessService exchangeAccessService)
        {
            _exchangeAccessIntegrationEventService = exchangeAccessIntegrationEventService ?? throw new ArgumentNullException(nameof(exchangeAccessIntegrationEventService));
            _marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _exchangeAccessService = exchangeAccessService ?? throw new ArgumentNullException(nameof(exchangeAccessService));
        }

        public async Task Handle(LiveRoundtripExitOrderSubmittedIntegrationEvent @event)
        {
            var market = await this._marketRepository.GetByCurrencyPairAsync(@event.BaseCurrency, @event.QuoteCurrency, @event.ExchangeId);
            var balance = await this._balanceRepository.GetByUsernameAsync(@event.Username);

            if (market == null || balance == null)
            {
                return;
            }

            var orderId = market.CreateSellOrder(
                balance,
                @event.BaseCurrency,
                @event.QuoteCurrency,
                @event.ExitPrice,
                @event.ExitAmount,
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

                await this._exchangeAccessIntegrationEventService.PublishThroughEventBusAsync(new LiveRoundtripExitOrderExecutedIntegrationEvent(
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
    }
}
