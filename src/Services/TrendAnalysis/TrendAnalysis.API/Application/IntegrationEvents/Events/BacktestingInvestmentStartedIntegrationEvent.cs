using CryptoTrading.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events
{
    public class BacktestingInvestmentStartedIntegrationEvent : IntegrationEvent
    {
        public BacktestingInvestmentStartedIntegrationEvent(string investmentId, string traceId, string investmentType, int exchangeId, string baseCurrency, string quoteCurrency, DateTime dateStarted, DateTime dateClosed, decimal initialBalance)
        {
            InvestmentId = investmentId;
            TraceId = traceId;
            InvestmentType = investmentType;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            DateStarted = dateStarted;
            DateClosed = dateClosed;
            InitialBalance = initialBalance;
        }

        public string InvestmentId { get; private set; }
        public string TraceId { get; private set; }

        public string InvestmentType { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public DateTime DateStarted { get; private set; }
        public DateTime DateClosed { get; private set; }
        public decimal InitialBalance { get; private set; }

    }
}
