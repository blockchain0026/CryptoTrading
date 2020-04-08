using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeOrderBookDataReceivedIntegrationEventHandler : IIntegrationEventHandler<ExchangeOrderBookDataReceivedIntegrationEvent>
    {
        private readonly IMarketRepository _marketRepository;

        public ExchangeOrderBookDataReceivedIntegrationEventHandler(IMarketRepository marketRepository)
        {
            this._marketRepository = marketRepository;
        }


        public async Task Handle(ExchangeOrderBookDataReceivedIntegrationEvent @event)
        {
            try
            {

                var baseCurrency = @event.OrderBook.BaseCurrency.ToUpper();
                var quoteCurrency = @event.OrderBook.QuoteCurrency.ToUpper();
                var exchangeId = @event.ExchangeId;

                var existingMarket = await this._marketRepository.GetByCurrencyPairAsync(baseCurrency.ToUpper(), quoteCurrency.ToUpper(), exchangeId);

                if (existingMarket == null)
                {
                    throw new KeyNotFoundException($"Market with symbol {baseCurrency + quoteCurrency} for exchange {exchangeId} not found");
                }


                existingMarket.UpdateEntireOrderBook(@event.OrderBook.Asks, @event.OrderBook.Bids);


                this._marketRepository.Update(existingMarket);

                var saved = false;
                var count = 0;




                while (!saved)
                {
                    try
                    {

                        await _marketRepository.UnitOfWork.SaveEntitiesAsync();
                        saved = true;

                        if (count > 0)
                        {
                            Console.WriteLine("Exception Solved: DbUpdateConcurrencyException. \n" +
                                "Try count:" + count.ToString());
                        }


                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Console.WriteLine("Try to solve exception : DbUpdateConcurrencyException. Time:" + DateTime.UtcNow.AddHours(8));
                        count++;

                        foreach (var entry in ex.Entries)
                        {
                            if (entry.Entity is Market)
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
                            else if (entry.Entity is MarketAsk)
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
                            else if (entry.Entity is MarketBid)
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
                            else
                            {
                                throw new NotSupportedException(
                                    "Don't know how to handle concurrency conflicts for "
                                    + entry.Metadata.Name);
                            }
                        }

                    }
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Handle Integration Event: ExchangeOrderBookDataReceivedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
