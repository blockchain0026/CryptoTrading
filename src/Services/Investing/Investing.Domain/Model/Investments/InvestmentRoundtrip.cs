using CryptoTrading.Services.Investing.Domain.Exceptions;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Investments
{
    public class InvestmentRoundtrip : Entity
    {
        public int RoundtripNumber { get; private set; }
        public string InvestmentId { get; private set; }
        public Market Market { get; private set; }
        public decimal EntryBalance { get; private set; }
        public decimal? ExitBalance { get; private set; }


        protected InvestmentRoundtrip()
        {

        }

        public InvestmentRoundtrip(int roundtripNumber, string investmentId, Market market, decimal entryBalance) : this()
        {
            RoundtripNumber = roundtripNumber;
            InvestmentId = investmentId ?? throw new InvestingDomainException("parameter not provided:" + nameof(investmentId));
            Market = market ?? throw new InvestingDomainException("parameter not provided:" + nameof(market));
            EntryBalance = entryBalance;
        }


        public void Exit(Roundtrip roundtrip)
        {
            if (roundtrip == null)
            {
                throw new InvestingDomainException("Roundtrip not provided;");
            }
            if (roundtrip.InvestmentId != this.InvestmentId)
            {
                throw new InvestingDomainException("InvestmentId not match.");
            }
            if (roundtrip.RoundtripNumber != this.RoundtripNumber)
            {
                throw new InvestingDomainException("RoundtripNumber not match.");
            }
            if (roundtrip.RoundtripStatus.Id != RoundtripStatus.Exit.Id && roundtrip.RoundtripStatus.Id != RoundtripStatus.ForceExit.Id)
            {
                throw new InvestingDomainException("The Roundtrip's status must be exit.");
            }
            if (this.IsFinished())
            {
                throw new InvestingDomainException("InvestmentRoundtrip is already exit.");
            }

            var exitBalance = roundtrip.ExitBalance;

            this.ExitBalance = exitBalance;
        }


        public bool IsFinished()
        {
            return this.ExitBalance != null;
        }

    }
}
