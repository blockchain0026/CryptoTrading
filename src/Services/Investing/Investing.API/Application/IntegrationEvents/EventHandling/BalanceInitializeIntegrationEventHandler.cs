using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.Investing.Domain.Model.Funds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.IntegrationEvents.EventHandling
{
    public class BalanceInitializeIntegrationEventHandler : IIntegrationEventHandler<BalanceInitializeIntegrationEvent>
    {
        private readonly IFundRepository _fundRepository;

        public BalanceInitializeIntegrationEventHandler(IFundRepository fundRepository)
        {
            _fundRepository = fundRepository ?? throw new ArgumentNullException(nameof(fundRepository));
        }

        public async Task Handle(BalanceInitializeIntegrationEvent @event)
        {
            var account = new Account(@event.Username);
            var existingFunds = await _fundRepository.GetByAccount(account);

            foreach (var asset in @event.Assets)
            {
                var matchFund = existingFunds.Where(f => f.Symbol.ToUpper() == asset.Symbol.ToUpper()).SingleOrDefault();

                if (matchFund == null)
                {

                    var fund = new Fund(new Account(@event.Username), asset.Symbol, asset.Total);

                    this._fundRepository.Add(fund);

                    await this._fundRepository.UnitOfWork.SaveEntitiesAsync();
                }
            }
        }
    }
}
