using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Exceptions;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Roundtrips
{
    public class Roundtrip : Entity, IAggregateRoot
    {
        #region Properties & Fields

        public string RoundtripId { get; private set; }
        public string InvestmentId { get; private set; }
        public int RoundtripNumber { get; private set; }
        public RoundtripStatus RoundtripStatus { get; private set; }
        private int _roundtripStatusId;

        public Market Market { get; private set; }

        public DateTime? EntryAt { get; private set; }
        public DateTime? ExitAt { get; private set; }

        public decimal EntryBalance { get; private set; }
        public decimal? ExitBalance { get; private set; }
        public decimal TargetPrice { get; private set; }
        public decimal StopLossPrice { get; private set; }
        public Transaction Transaction { get; private set; }

        #endregion



        #region Constructor
        protected Roundtrip()
        {
        }

        public Roundtrip(string investmentId, int roundtripNumber, Market market, decimal entryBalance, decimal entryPrice, decimal targetPrice, decimal stopLossPrice, DateTime adviceCreationDate) : this()
        {
            this.RoundtripId = Guid.NewGuid().ToString();
            this.InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
            this.RoundtripNumber = roundtripNumber >= 0 ? roundtripNumber : throw new ArgumentOutOfRangeException(nameof(roundtripNumber));
            this._roundtripStatusId = RoundtripStatus.EntryOrderSubmitted.Id;
            this.Market = market ?? throw new ArgumentNullException(nameof(market));
            this.EntryBalance = entryBalance > 0 ? entryBalance : throw new ArgumentNullException(nameof(entryBalance));
            this.TargetPrice = targetPrice >= 0 ? targetPrice : throw new ArgumentNullException(nameof(targetPrice));
            this.StopLossPrice = stopLossPrice >= 0 ? stopLossPrice : throw new ArgumentNullException(nameof(stopLossPrice));
            this.Transaction = new Transaction();

            this.AddDomainEvent(new RoundtripEntryOrderSubmittedDomainEvent(
                this.InvestmentId,
                this.RoundtripId,
                RoundtripStatus.From(this._roundtripStatusId),
                this.Market,
                this.EntryBalance,
                entryPrice,
                adviceCreationDate
                ));
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        #endregion



        #region Functions
        public void OrderFilled(decimal filledAmount, decimal filledPrice, DateTime dateFilled, decimal feePercent = 0)
        {
            if (this._roundtripStatusId == RoundtripStatus.Exit.Id || this._roundtripStatusId == RoundtripStatus.Entry.Id)
            {
                throw new InvestingDomainException("A roundtrip's order should not filled before submitting order.");
            }

            if (this._roundtripStatusId == RoundtripStatus.EntryOrderSubmitted.Id)
            {
                var fee = Math.Round(filledAmount * feePercent, 8);
                var buyAmount = filledAmount - fee;

                this.Transaction = this.Transaction.BuyOrderFilled(buyAmount, filledPrice);
                this.EntryAt = dateFilled;
                this._roundtripStatusId = RoundtripStatus.Entry.Id;

                this.AddDomainEvent(new RoundtripEntryDomainEvent(
                    this.InvestmentId,
                    this.RoundtripId,
                    RoundtripStatus.From(this._roundtripStatusId),
                    this.Market,
                    this.EntryAt ?? throw new InvestingDomainException("Param \"Entry At\" missing."),
                    this.Transaction
                    ));
            }
            else if (this._roundtripStatusId == RoundtripStatus.ExitOrderSubmitted.Id)
            {
                this.Transaction = this.Transaction.SellOrderFilled(filledAmount, filledPrice);
                this.ExitAt = dateFilled;

                var originalBalance = this.EntryBalance - (Math.Round((decimal)this.Transaction.BuyAmount / (1 - feePercent), 8) * this.Transaction.BuyPrice);

                var fee = Math.Round(filledAmount * filledPrice * feePercent, 8);
                this.ExitBalance = originalBalance + (filledPrice * filledAmount) - fee;

                this._roundtripStatusId = RoundtripStatus.Exit.Id;

                this.AddDomainEvent(new RoundtripExitDomainEvent(this));
            }
            else
            {
                throw new InvestingDomainException("Order filled in wrong state at roundtrip.");
            }

        }

        public void PriceChanged(decimal currentPrice, DateTime dateChanged, decimal? backtestingCandleHighestPrice = null, decimal? backtestingCandleLowestPrice = null, decimal? nextTargetPrice = null)
        {
            if (this._roundtripStatusId != RoundtripStatus.Entry.Id)
            {
                throw new InvestingDomainException("No need to notify the price for a roundtrip that is not in entry status.");
            }


            if (backtestingCandleHighestPrice != null && backtestingCandleLowestPrice != null && nextTargetPrice != null)
            {
                if (backtestingCandleLowestPrice <= this.StopLossPrice)
                {
                    currentPrice = (decimal)backtestingCandleLowestPrice;
                }
                else if (backtestingCandleHighestPrice >= this.TargetPrice)
                {
                    this.MoveTargetPrice((decimal)nextTargetPrice);
                    return;
                }
                else
                    return;
            }

            if (currentPrice <= this.StopLossPrice)
            {

                //var slipPercent = 0.98M;
                var slipPercent = 1.00M;

                this.Exit(
                    currentPrice * slipPercent,
                    dateChanged);
            }
            else if (currentPrice >= this.TargetPrice)
            {
                this.AddDomainEvent(new RoundtripTargetPriceHitDomainEvent(
                    this.RoundtripId,
                    this.InvestmentId,
                    this.RoundtripNumber,
                    this.TargetPrice,
                    this.Market
                    ));
            }
        }


        public void Exit(decimal exitPrice, DateTime adviceCreationDate)
        {
            if (this._roundtripStatusId != RoundtripStatus.Entry.Id)
            {
                throw new InvestingDomainException("A roundtrip can only exit in Entry status.");
            }
            //Only sell at stop loss price, let the market decide.
            if (exitPrice > this.StopLossPrice)
            {
                return;
            }
            this._roundtripStatusId = RoundtripStatus.ExitOrderSubmitted.Id;

            this.AddDomainEvent(new RoundtripExitOrderSubmittedDomainEvent(
                this,
                this.InvestmentId,
                this.RoundtripId,
                RoundtripStatus.From(this._roundtripStatusId),
                this.Market,
                this.Transaction.BuyAmount ?? throw new InvestingDomainException("Transaction buy amount missing."),
                exitPrice,
                adviceCreationDate
                ));
        }

        public void MoveTargetPrice(decimal nextTargetPrice)
        {
            if (this._roundtripStatusId != RoundtripStatus.Entry.Id)
            {
                throw new InvestingDomainException("A roundtrip can only move target price in Entry status.");
            }

            decimal stopProfit;

            /*stopProfit = this.TargetPrice * 0.97M < currentPrice ?
                this.TargetPrice * 0.97M :
                currentPrice * 0.97M;*/

            stopProfit = this.TargetPrice;

            this.TargetPrice = nextTargetPrice;

            this.SetStopProfit(stopProfit);
        }



        public void ForceSelling()
        {
            if (this._roundtripStatusId == RoundtripStatus.Exit.Id)
            {
                throw new InvestingDomainException("Roundtrip already exit, nothing could be selled.");
            }
            if (this._roundtripStatusId == RoundtripStatus.ForceExit.Id || this._roundtripStatusId == RoundtripStatus.ForceSell.Id)
            {
                throw new InvestingDomainException("Force selling in wrong state ata roundtrip.");
            }


            this.AddDomainEvent(new RoundtripForcedSellingDomainEvent(
                this.RoundtripId,
                RoundtripStatus.From(this._roundtripStatusId)
                ));

            this._roundtripStatusId = RoundtripStatus.ForceSell.Id;
        }

        public void ForceSellingSuccess(decimal sellAmount, decimal sellPrice, DateTime dateSell)
        {
            if (this._roundtripStatusId != RoundtripStatus.ForceSell.Id)
            {
                throw new InvestingDomainException("Force selling success should happend after force selling and before success.");
            }

            this.Transaction = this.Transaction.SellOrderFilled(sellAmount, sellPrice);

            this.ExitAt = dateSell;
            this.ExitBalance = sellPrice * sellAmount;

            this._roundtripStatusId = RoundtripStatus.ForceExit.Id;

            this.AddDomainEvent(new RoundtripForcedExitDomainEvent(
                this
                ));
        }

        public RoundtripStatus GetStatus()
        {
            return RoundtripStatus.From(this._roundtripStatusId);
        }
        #endregion



        #region Private Functions
        private void SetStopProfit(decimal stopPrice)
        {
            this.StopLossPrice = stopPrice;
        }
        #endregion
    }
}
