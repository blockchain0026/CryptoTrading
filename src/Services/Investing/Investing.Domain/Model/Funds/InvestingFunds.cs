using CryptoTrading.Services.Investing.Domain.Exceptions;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.SeedWork;

namespace CryptoTrading.Services.Investing.Domain.Model.Funds
{
    public class InvestingFund : Entity
    {
        public decimal InitialQuantity { get; private set; }
        public decimal CurrentQuantity { get; private set; }
        public decimal? EndQuantity { get; private set; }
        public string InvestmentId { get; private set; }
        public InvestingFundStatus InvestingFundStatus { get; private set; }
        private int _investingFundStatusId;

        public InvestingFund(string investmentId, decimal initialQuantity)
        {
            InvestmentId = investmentId ?? throw new InvestingDomainException("parameter not provided:" + nameof(investmentId));
            InitialQuantity = initialQuantity > 0 ? initialQuantity : throw new InvestingDomainException("The initial quantity should be large than 0 for investing fund.");
            this.CurrentQuantity = this.InitialQuantity;

            this._investingFundStatusId = InvestingFundStatus.Initialize.Id;
        }

        public void UpdateInitialFund(decimal quantity)
        {
            this.InitialQuantity = quantity > 0 ? quantity : throw new InvestingDomainException("The initial quantity should be large than 0 for investing fund."); ;
        }

        public void CurrentQuantityUpdate(decimal quantity)
        {
            this.CurrentQuantity = quantity >= 0 ? quantity : throw new InvestingDomainException("The current quantity shouldn't be less than 0 for investing fund.");
        }

        public void ForceEnded()
        {
            if (this._investingFundStatusId == InvestingFundStatus.Ended.Id)
            {
                throw new InvestingDomainException("Investing fund is already ended.");
            }

            this.EndQuantity = CurrentQuantity;
            this.CurrentQuantity = 0;
            this._investingFundStatusId = InvestingFundStatus.Ended.Id;
        }

        public void InvestmentClosed(Investment investment)
        {
            if (investment.InvestmentId != this.InvestmentId)
            {
                throw new InvestingDomainException("InvestmentId not match.");
            }
            if (investment.InvestmentStatus.Id != InvestmentStatus.Closed.Id)
            {
                throw new InvestingDomainException("The investment provided is in wrong status for the operaton.");
            }

            var endedQuantity = investment.EndBalance;

            if (endedQuantity == null)
            {
                throw new InvestingDomainException("The ended balance is missing from provided investment.");
            }

            this.EndQuantity = endedQuantity;

            this._investingFundStatusId = InvestingFundStatus.Ended.Id;
        }

        public InvestingFundStatus GetStatus()
        {
            return InvestingFundStatus.From(this._investingFundStatusId);
        }
    }
}