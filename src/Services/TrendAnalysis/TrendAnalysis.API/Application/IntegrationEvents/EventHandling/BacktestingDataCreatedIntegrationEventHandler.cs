using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.EventHandling
{
    public class BacktestingDataCreatedIntegrationEventHandler : IIntegrationEventHandler<BacktestingDataCreatedIntegrationEvent>
    {
        private readonly ITraceRepository _traceRepository;
        private readonly ITrendAnalysisIntegrationEventService _trendAnalysisIntegrationEventService;


        public BacktestingDataCreatedIntegrationEventHandler(ITraceRepository traceRepository, ITrendAnalysisIntegrationEventService trendAnalysisIntegrationEventService)
        {
            this._traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            this._trendAnalysisIntegrationEventService = trendAnalysisIntegrationEventService ?? throw new ArgumentNullException(nameof(trendAnalysisIntegrationEventService));
        }

        public async Task Handle(BacktestingDataCreatedIntegrationEvent @event)
        {
            var traceId = @event.TraceId;
            var candlesData = @event.Candles;


            var existingTrace = await _traceRepository.GetByTraceIdAsync(traceId);

            if (existingTrace == null)
            {
                return;
            }
            if (existingTrace.Investment.InvestmentId != @event.InvestmentId)
            {
                return;
            }
            if (existingTrace.Market.BaseCurrency != @event.BaseCurrency || existingTrace.Market.QuoteCurrency != @event.QuoteCurrency)
            {
                return;
            }

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            var from = dtDateTime.AddSeconds(@event.From);
            var to = dtDateTime.AddSeconds(@event.To);

            while (from.Second != 0)
            {
                from = from.AddSeconds(1);
            }

            var timeService = new BacktestingTimeService();
            timeService.SetCurrentDateTime(from);

            var currentDateTime = timeService.GetCurrentDateTime();
            var pastCandles = new Dictionary<string, List<Candle>>();

            foreach (var key in candlesData.Keys)
            {
                pastCandles.Add(key, new List<Candle>());


                var warmingCandles = candlesData[key].Where(c => c.Timestamp < currentDateTime).OrderBy(c => c.Timestamp).ToList();

                if (warmingCandles.Any())
                {
                    
                    warmingCandles.Remove(warmingCandles.Last());
                    
                    foreach (var candle in warmingCandles)
                    {
                        pastCandles[key].Add(new Candle(
                                    candle.Timestamp,
                                    candle.High,
                                    candle.Low,
                                    candle.Open,
                                    candle.Close,
                                    candle.Volume));
                    }
                }

            }

            try
            {
                while (currentDateTime <= to)
                {


                    /*await this._trendAnalysisIntegrationEventService.PublishThroughEventBusAsync(new BacktestingTimePassIntegrationEvent(
                        currentDateTime,
                        traceId,
                        @event.InvestmentId
                        ));*/

                    foreach (var candles in candlesData)
                    {

                        var currentCandleIndex = candles.Value.FindIndex(c => c.Timestamp == currentDateTime);

                        Models.Candle matchedCandle = null;

                        if (currentCandleIndex != -1)
                        {
                            matchedCandle = candles.Value[currentCandleIndex - 1];
                        }

                        if (matchedCandle != null)
                        {
                            /*if (matchedCandle.Timestamp == new DateTime(2018, 12, 8, 12, 0, 0))
                            {
                                var testPause = 0;
                            }*/

                            if (pastCandles.TryGetValue(candles.Key, out List<Candle> existingPastCandles))
                            {
                                existingPastCandles.Add(new Candle(
                                    matchedCandle.Timestamp,
                                    matchedCandle.High,
                                    matchedCandle.Low,
                                    matchedCandle.Open,
                                    matchedCandle.Close,
                                    matchedCandle.Volume));

                                if (candles.Key == CandlePeriod.OneMinute.Name)
                                {

                                    var runningCandle = candles.Value.Where(c => c.Timestamp == currentDateTime.AddMinutes(1)).SingleOrDefault();
                                    if (runningCandle != null)
                                    {
                                        var highestPrice = runningCandle.High;
                                        var lowestPrice = runningCandle.Low;

                                        //Only need next target price when current price cross current target price.
                                        var targetPrice = Trace.CalculateTargetPrice(existingPastCandles, highestPrice);


                                        await this._trendAnalysisIntegrationEventService
                                            .PublishThroughEventBusAsync(new BacktestingPriceChangedIntegrationEvent(
                                                traceId,
                                                @event.InvestmentId,
                                                currentDateTime,
                                                highestPrice,
                                                lowestPrice,
                                                targetPrice
                                                ));
                                    }
                                }


                                var processingTrace = await this._traceRepository.GetByTraceIdAsync(traceId);
                                var indicatorService = new IndicatorService();


                                processingTrace.CandleUpdated(existingPastCandles, CandlePeriod.FromName(candles.Key), timeService, new IndicatorService());

                                this._traceRepository.Update(processingTrace);
                                
                                await this._traceRepository.UnitOfWork.SaveEntitiesAsync();

                                pastCandles[candles.Key] = existingPastCandles;
                            }
                        }


                    }

                    currentDateTime = currentDateTime.AddMinutes(1);
                    timeService.SetCurrentDateTime(currentDateTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Integration Event: BacktestingDataCreatedIntegrationEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }

            /*currentDateTime = currentDateTime.AddMinutes(-1);
            timeService.SetCurrentDateTime(currentDateTime);*/

            var toFinishedTrace = await this._traceRepository.GetByTraceIdAsync(traceId);
            toFinishedTrace.CloseTracing(timeService);

            this._traceRepository.Update(toFinishedTrace);
            await this._traceRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
