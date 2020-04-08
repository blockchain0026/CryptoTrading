using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Exceptions;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeBalanceDataReceivedIntegrationEventHandler : IIntegrationEventHandler<ExchangeBalanceDataReceivedIntegrationEvent>
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly IExchangeAccessIntegrationEventService _exchangeAccessIntegrationEventService;

        public ExchangeBalanceDataReceivedIntegrationEventHandler(IBalanceRepository balanceRepository, IExchangeAccessIntegrationEventService exchangeAccessIntegrationEventService)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _exchangeAccessIntegrationEventService = exchangeAccessIntegrationEventService ?? throw new ArgumentNullException(nameof(exchangeAccessIntegrationEventService));
        }

        public async Task Handle(ExchangeBalanceDataReceivedIntegrationEvent @event)
        {
            try
            {
                var balanceId = @event.BalanceId;
                var exchangeId = @event.ExchangeId;

                var existingBalance = await this._balanceRepository.GetByBalanceIdAsync(balanceId);

                if (existingBalance == null)
                {
                    throw new KeyNotFoundException($"Balance with Id {balanceId} for exchange {exchangeId} not found");
                }

                foreach (var asset in @event.Assets)
                {
                    try
                    {
                        if (asset.Total != 0)
                        {
                            existingBalance.AssetUpdated(asset.Symbol, asset.Available, asset.Locked);
                        }
                    }
                    catch (ExchangeAccessDomainException ex)
                    {
                        if (ex.Message == $"Asset with symbol {asset.Symbol} is not found.")
                        {
                            existingBalance.AddAsset(asset.Symbol, asset.Available, asset.Locked);
                        }
                    }
                }


                this._balanceRepository.Update(existingBalance);

                var saved = false;
                var count = 0;

                while (!saved)
                {
                    try
                    {

                        await _balanceRepository.UnitOfWork.SaveEntitiesAsync();
                        saved = true;

                        if (count > 0)
                        {
                            Console.WriteLine("Exception Solved: DbUpdateConcurrencyException. \n" +
                                "Try count:" + count.ToString());
                        }


                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Console.WriteLine("Try to solve exception : DbUpdateConcurrencyException. Time (UTC+8):" + DateTime.UtcNow.AddHours(8));
                        count++;

                        foreach (var entry in ex.Entries)
                        {

                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues != null ? databaseValues[property] : proposedValue;

                                // TODO: decide which value should be written to database
                                proposedValues[property] = databaseValue;
                            }


                        }

                    }
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: ExchangeBalanceDataReceivedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }

        }
    }
}
