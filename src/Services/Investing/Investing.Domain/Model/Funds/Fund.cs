using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Exceptions;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Funds
{
    public class Fund : Entity, IAggregateRoot
    {
        #region Properties & Fields

        public string FundId { get; private set; }
        public Account Account { get; private set; }
        public string Symbol { get; private set; }
        public decimal FreeBalance { get; private set; }


        private readonly List<InvestingFund> _investingFunds;
        public IReadOnlyCollection<InvestingFund> InvestingFunds => _investingFunds;


        #endregion



        #region Constructor
        protected Fund()
        {
            this._investingFunds = new List<InvestingFund>();
        }


        public Fund(Account account, string symbol, decimal totalBalance) : this()
        {
            this.FundId = Guid.NewGuid().ToString();

            this.Account = account ?? throw new ArgumentNullException(nameof(account));
            this.Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            this.FreeBalance = totalBalance;
        }


        #endregion



        #region Functions
        public void WithdrawOut(decimal withdrawAmount)
        {
            if (withdrawAmount > this.FreeBalance)
            {
                var extraAmount = withdrawAmount - this.FreeBalance;

                if (extraAmount > this.CalculateInvestingFundTotalCurrentAmounts())
                {
                    throw new InvestingDomainException("Withdraw amount should be equal or less than total balance.");
                }

                var unfinished = this._investingFunds.Where(i => i.InvestingFundStatus.Id == InvestingFundStatus.Initialize.Id);

                foreach (var fund in unfinished)
                {
                    this.CancelFunding(fund.InvestmentId);

                    if (withdrawAmount <= this.FreeBalance)
                    {
                        break;
                    }
                }
            }

            this.FreeBalance = this.FreeBalance - withdrawAmount;
        }

        public void DepositIn(decimal depositAmount)
        {
            this.FreeBalance += depositAmount > 0 ? depositAmount : throw new InvestingDomainException("Deposit amount for fund should be larger than 0.");
        }

        public void AllocateFundsFor(Investment investment, decimal quantity)
        {
            if (quantity >= this.FreeBalance)
            {
                throw new InvestingDomainException("There is no enough fund to allocate.");
            }
            if (investment == null)
            {
                throw new InvestingDomainException("The investment is not provided.");
            }
            if (investment.InvestmentStatus.Id != InvestmentStatus.Prepare.Id)
            {
                throw new InvestingDomainException("Could not allocate funds for a investment that is not in prepare state.");
            }
            if (!investment.Account.Equals(this.Account))
            {
                throw new InvestingDomainException("The fund account must be the same with the investment account.");
            }
            if (investment.Market.QuoteCurrency != this.Symbol)
            {
                throw new InvestingDomainException("The funding coin's symbol must match the investment's market quote currency.");
            }



            if (this.GetFundingQuantity(investment.InvestmentId).Any())
            {
                var existingFund = this._investingFunds.Where(i => i.InvestmentId == investment.InvestmentId).SingleOrDefault();

                this.FreeBalance += existingFund.InitialQuantity;

                existingFund.UpdateInitialFund(quantity);

                this.FreeBalance -= existingFund.InitialQuantity;
            }
            else
            {
                this._investingFunds.Add(new InvestingFund(investment.InvestmentId, quantity));
                this.FreeBalance -= quantity;
            }


            this.AddDomainEvent(new InvestmentFundAllocatedDomainEvent(
                this,
                investment.InvestmentId,
                this.GetFundingQuantity(investment.InvestmentId).First()
                ));
        }

        public void InvestmentClosed(Investment investment)
        {
            if (investment == null)
            {
                throw new InvestingDomainException("The investment is not provided.");
            }


            var existingFund = this._investingFunds.Where(i => i.InvestmentId == investment.InvestmentId).SingleOrDefault();

            if (existingFund == null)
            {
                throw new InvestingDomainException("No existing fund match the investment's Id");
            }

            existingFund.InvestmentClosed(investment);

            this.ReleaseFunds(existingFund.InvestmentId);
        }

        public void CancelFunding(string investmentId = null)
        {
            if (investmentId == null)
            {
                foreach (var fund in this.GetUnfinishedFunds())
                {
                    fund.ForceEnded();
                    this.ReleaseFunds(fund.InvestmentId);

                    this.AddDomainEvent(new InvestmentFundingCanceledDomainEvent(
                        this,
                        fund.InvestmentId
                        ));
                }
            }
            else
            {
                var toRemove = this._investingFunds.Where(i => i.InvestmentId == investmentId).SingleOrDefault();

                if (toRemove == null)
                {
                    throw new InvestingDomainException($"Fund to remove not found with Id {investmentId}");
                }

                toRemove.ForceEnded();
                this.ReleaseFunds(toRemove.InvestmentId);

                this.AddDomainEvent(new InvestmentFundingCanceledDomainEvent(
                    this,
                    toRemove.InvestmentId
                    ));
            }
        }

        public void InvestmentFundUpdated(Investment investment)
        {
            if (investment == null)
            {
                throw new InvestingDomainException("Investment argument is not provided.");
            }

            var existingInvestment = this._investingFunds.Where(i => i.InvestmentId == investment.InvestmentId).SingleOrDefault();

            if (existingInvestment == null)
            {
                throw new InvestingDomainException($"No fund for investment {investment.InvestmentId} founded.");
            }

            existingInvestment.CurrentQuantityUpdate(investment.CurrentBalance);
        }

        public IEnumerable<decimal> GetFundingQuantity(string investmentId = null)
        {
            var funds = new List<decimal>();

            if (investmentId == null)
            {

                foreach (var fund in this._investingFunds)
                {
                    if (fund.InvestingFundStatus.Id != InvestingFundStatus.Ended.Id)
                    {
                        funds.Add(fund.InitialQuantity);
                    }
                }

            }
            else
            {
                var fund = this._investingFunds.Where(f => f.InvestmentId == investmentId).SingleOrDefault();

                if (fund != null)
                {
                    if (fund.GetStatus().Id != InvestingFundStatus.Ended.Id)
                    {
                        funds.Add(fund.InitialQuantity);
                    }
                }
            }

            return funds;
        }
        #endregion



        #region Private Functions
        private decimal CalculateInvestingFundTotalCurrentAmounts()
        {
            decimal totalAmounts = 0;

            foreach (var fund in this._investingFunds)
            {
                totalAmounts += fund.CurrentQuantity;
            }

            return totalAmounts;
        }
        private void ReleaseFunds(string investmentId)
        {
            var toRelease = this._investingFunds.Where(i => i.InvestmentId == investmentId).SingleOrDefault();
            if (toRelease == null)
            {
                throw new InvestingDomainException("Fund not found.");
            }

            var endQuantity = toRelease.EndQuantity ?? throw new InvestingDomainException("End quantity missing when releasing fund.");
            this.FreeBalance += endQuantity;
        }

        private void RemoveEndedFunds()
        {
            var toRemoveFunds = this._investingFunds.Where(i => i.InvestingFundStatus.Id == InvestingFundStatus.Ended.Id);

            if (toRemoveFunds.Any())
            {
                foreach (var fund in toRemoveFunds)
                {
                    this._investingFunds.Remove(fund);
                }
            }
        }

        private IEnumerable<InvestingFund> GetUnfinishedFunds()
        {
            return this._investingFunds.Where(i => i.InvestingFundStatus.Id == InvestingFundStatus.Initialize.Id);
        }
        #endregion
    }
}
