using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Exceptions;
using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoTrading.Services.Investing.Domain.Model.Investments
{

    public class Investment : Entity, IAggregateRoot
    {
        #region Properties & Fields

        public string InvestmentId { get; private set; }
        public Trace Trace { get; private set; }
        public Account Account { get; private set; }

        public InvestmentStatus InvestmentStatus { get; private set; }
        private int _investmentStatusId;
        public InvestmentType InvestmentType { get; private set; }
        private int _investmentTypeId;


        public Market Market { get; private set; }

        public DateTime? DateStarted { get; private set; }
        public DateTime? DateClosed { get; private set; }

        public decimal InitialBalance { get; private set; }
        public decimal CurrentBalance { get; private set; }
        public decimal? EndBalance { get; private set; }



        private readonly List<InvestmentRoundtrip> _investmentRoundtrips;
        public IReadOnlyCollection<InvestmentRoundtrip> InvestmentRoundtrips => _investmentRoundtrips;

        #endregion



        #region Constructor

        protected Investment()
        {
            this._investmentRoundtrips = new List<InvestmentRoundtrip>();
        }

        private Investment(string investmentId, int investmentTypeId, int exchangeId, string baseCurrency, string quoteCurrency, Trace trace, Account account) : this()
        {
            this.InvestmentId = investmentId ?? throw new ArgumentNullException(nameof(investmentId));
            this._investmentTypeId = investmentTypeId;
            this.Market = new Market(
                exchangeId,
                baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency)),
                quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency))
                );
            this.Trace = trace ?? throw new ArgumentNullException(nameof(trace));
            this.Account = account ?? throw new ArgumentNullException(nameof(account));

            this._investmentStatusId = InvestmentStatus.Prepare.Id;
        }

        #endregion




        #region Functions

        public static Investment FromType(InvestmentType investmentType, int exchangeId, string baseCurrency, string quoteCurrency, Account account)
        {
            if (investmentType == null)
            {
                throw new InvestingDomainException("InvestmentType must be provided when creating investment.");
            }

            return new Investment(
                Guid.NewGuid().ToString(),
                investmentType.Id,
                exchangeId,
                baseCurrency,
                quoteCurrency,
                new Trace(),
                account);
        }

        public void ChangeMarket(int exchangeId, string baseCurrency, string quoteCurrency)
        {
            if (this._investmentStatusId != InvestmentStatus.Prepare.Id)
            {
                throw new InvestingDomainException("Investment's market setting can only be change at prepare state.");
            }

            this.Market = new Market(exchangeId, baseCurrency, quoteCurrency);
        }

        public void ChangeInvestmentType(InvestmentType investmentType)
        {
            if (this._investmentStatusId != InvestmentStatus.Prepare.Id)
            {
                throw new InvestingDomainException("Investment's investmentType can only be change at prepare state.");
            }
            if (investmentType == null)
            {
                throw new InvestingDomainException("InvestmentType must be provided when changing investment type.");
            }


            if (this._investmentTypeId == InvestmentType.Backtesting.Id)
            {
                this.DateStarted = null;
                this.DateClosed = null;
                this.InitialBalance = 0;
                this.CurrentBalance = 0;
            }

            this._investmentTypeId = investmentType.Id;
        }

        public void SetPeriodForBacktesting(DateTime from, DateTime to)
        {
            if (this._investmentTypeId != InvestmentType.Backtesting.Id)
            {
                throw new InvestingDomainException("Could not set period for backtesting, because this is not a backtesting investment.");
            }
            if (this._investmentStatusId != InvestmentStatus.Prepare.Id)
            {
                throw new InvestingDomainException("Investment's backtesting setting can only be change at prepare state.");
            }

            if (to > DateTime.UtcNow)
            {
                throw new InvestingDomainException("Investment's backtesting period must not pass currenct time. ");
            }

            if (from < new DateTime(2013, 1, 1))
            {
                throw new InvestingDomainException("Investment's backtesting period must not early than 2013/01/01. ");
            }

            this.DateStarted = from;
            this.DateClosed = to;
        }

        public void Funded(Fund fund)
        {
            if (fund == null)
            {
                throw new InvestingDomainException("Fund must provide when set fund for investment.");
            }
            if (this._investmentStatusId != InvestmentStatus.Prepare.Id)
            {
                throw new InvestingDomainException("Investment's fund setting can only be change at prepare state.");
            }
            if (this._investmentTypeId != InvestmentType.Live.Id)
            {
                throw new InvestingDomainException("The investment type must be live for allocating fund.");
            }

            if (!fund.Account.Equals(this.Account))
            {
                throw new InvestingDomainException("The fund account must be the same with the investment account.");
            }


            var quantityList = fund.GetFundingQuantity(this.InvestmentId);

            if (!quantityList.Any())
            {
                throw new InvestingDomainException("No allocated fund found in the fund provided.");
            }

            this.InitialBalance = quantityList.First();
        }

        public void SetSimulateFund(decimal quantity)
        {
            if (this._investmentStatusId != InvestmentStatus.Prepare.Id)
            {
                throw new InvestingDomainException("Investment's fund setting can only be change at prepare state.");
            }
            if (this._investmentTypeId == InvestmentType.Live.Id)
            {
                throw new InvestingDomainException("The investment type must be paper or backtesting for simulating fund.");
            }

            if (quantity <= 0)
            {
                throw new InvestingDomainException("The fund quantity must be larger than 0 for simulating.");
            }

            this.InitialBalance = quantity;
        }

        public void Settle()
        {
            if (this._investmentStatusId != InvestmentStatus.Prepare.Id)
            {
                throw new InvestingDomainException("Investment can only be settle at prepare state.");
            }

            if (this._investmentTypeId == InvestmentType.Backtesting.Id)
            {
                if (this.DateStarted == null || this.DateClosed == null)
                {
                    throw new InvestingDomainException("The test period for backtesting must be settled before settlement.");
                }
                if (this.InitialBalance <= 0)
                {
                    throw new InvestingDomainException("The initial fund for backtesting must be settled before settlement.");
                }
            }
            else if (this._investmentTypeId == InvestmentType.Paper.Id)
            {
                if (this.DateStarted != null || this.DateClosed != null)
                {
                    throw new InvestingDomainException("The start time or close time for paper trading must be empty before settlement.");
                }
                if (this.InitialBalance <= 0)
                {
                    throw new InvestingDomainException("The initial fund for paper trading must be settled before settlement.");
                }
            }
            else if (this._investmentTypeId == InvestmentType.Live.Id)
            {
                if (this.DateStarted != null || this.DateClosed != null)
                {
                    throw new InvestingDomainException("The start time or close time for live trading must be empty before settlement.");
                }
                if (this.InitialBalance <= 0)
                {
                    throw new InvestingDomainException("The initial fund for live trading must be settled before settlement.");
                }
            }

            this._investmentStatusId = InvestmentStatus.Settled.Id;

            this.AddDomainEvent(new InvestmentSettledDomainEvent(this));
        }

        public void TraceArranged(Trace trace)
        {
            if (this._investmentStatusId != InvestmentStatus.Settled.Id)
            {
                throw new InvestingDomainException("Trace should be arranged after investment settled and before ready.");
            }

            this.Trace = trace ?? throw new InvestingDomainException("The aranged trace must not be null.");

            this._investmentStatusId = InvestmentStatus.Ready.Id;

            this.AddDomainEvent(
                new InvestmentReadyDomainEvent(
                    this,
                    this.Trace,
                    this.InvestmentStatus));
        }

        public void Start()
        {
            if (this._investmentStatusId != InvestmentStatus.Ready.Id)
            {
                throw new InvestingDomainException("Investment can only start after ready.");
            }

            if (this._investmentTypeId != InvestmentType.Backtesting.Id)
            {
                var utcNow = DateTime.UtcNow;
                this.DateStarted = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second);
            }

            this.CurrentBalance = this.InitialBalance;

            this._investmentStatusId = InvestmentStatus.Started.Id;

            this.AddDomainEvent(
                new InvestmentStartedDomainEvent(this));
        }

        public void BuyingAdvice(string traceId, decimal executePrice, decimal targetPrice, DateTime dateCreated)
        {
            if (this._investmentStatusId != InvestmentStatus.Started.Id)
            {
                throw new InvestingDomainException("Investment can only be advice after started and before close.");
            }
            if (traceId == null || traceId != this.Trace.TraceId)
            {
                throw new InvestingDomainException("The traceId of advice must be provided and match the investment's traceId.");
            }

            if (executePrice <= 0 || targetPrice <= 0)
            {
                throw new InvestingDomainException("The execute price and the target price must be larger than 0.");
            }
            if (targetPrice <= executePrice)
            {
                throw new InvestingDomainException("The target price should be larger than the execute price.");

            }


            var runningRoundtrip = this._investmentRoundtrips.Where(r => r.IsFinished() == false).SingleOrDefault();

            //Only allow one running roundtrip.
            if (runningRoundtrip != null)
            {
                return;
            }

            var newRoundtrip = new InvestmentRoundtrip(
                this._investmentRoundtrips.Count + 1,
                this.InvestmentId,
                this.Market,
                this.CurrentBalance
                );

            this._investmentRoundtrips.Add(newRoundtrip);

            this.UpdateCurrentBalance(this.CurrentBalance - newRoundtrip.EntryBalance);

            var stopLossPrice = (1 - (((targetPrice / executePrice) - 1) / 2)) * executePrice;



            this.AddDomainEvent(new InvestmentRoundtripCreatedDomainEvent(
                this.InvestmentId,
                newRoundtrip.RoundtripNumber,
                dateCreated,
                newRoundtrip.Market,
                newRoundtrip.EntryBalance,
                executePrice,
                targetPrice,
                stopLossPrice
                ));
        }

        public void SellingAdvice(string traceId, decimal executePrice, DateTime dateCreated)
        {
            if (this._investmentStatusId != InvestmentStatus.Started.Id)
            {
                throw new InvestingDomainException("Investment can only be advice after started and before close.");
            }
            if (traceId == null || traceId != this.Trace.TraceId)
            {
                throw new InvestingDomainException("The traceId of advice must be provided and match the investment's traceId.");
            }

            if (executePrice <= 0)
            {
                throw new InvestingDomainException("The execute price must be larger than 0.");
            }


            var runningRoundtrip = this._investmentRoundtrips.Where(r => r.IsFinished() == false).SingleOrDefault();

            if (runningRoundtrip != null)
            {
                this.AddDomainEvent(new InvestmentSellingAdviceCreatedDomainEvent(
                    this.InvestmentId,
                    runningRoundtrip.RoundtripNumber,
                    executePrice,
                    dateCreated
                    ));
            }
        }

        public void RoundtripExit(Roundtrip roundtrip)
        {
            if (this._investmentStatusId != InvestmentStatus.Started.Id && this._investmentStatusId != InvestmentStatus.ForcedClosing.Id)
            {
                throw new InvestingDomainException("A roundtrip should exist after started and before close.");
            }
            if (roundtrip.InvestmentId != this.InvestmentId)
            {
                throw new InvestingDomainException("The investmentId of roundtrip must match the investment's Id.");
            }


            var roundtripNumber = roundtrip.RoundtripNumber;

            var existingRoundtrip = this._investmentRoundtrips.Where(r => r.RoundtripNumber == roundtripNumber).SingleOrDefault();

            if (existingRoundtrip == null)
            {
                throw new InvestingDomainException($"The roundtrip with number {roundtrip.RoundtripNumber} was not found.");
            }

            existingRoundtrip.Exit(roundtrip);

            this.UpdateCurrentBalance(
                this.CurrentBalance +
                existingRoundtrip.ExitBalance ?? throw new InvestingDomainException("Exit balance missing."));


            //Close investment if the fund is almost lost.
            if (this.CurrentBalance <= this.InitialBalance * 0.1M)
            {
                this.Close();
            }

            //Check if this is forced closing.
            if (this._investmentStatusId == InvestmentStatus.ForcedClosing.Id)
            {
                this.Close(forceClose: true);
            }
        }

        public void Close(bool forceClose = false)
        {
            if (this._investmentStatusId == InvestmentStatus.ForcedClosing.Id)
            {
                if (this.HasUnfinishedRoundtrip())
                {
                    return;
                }
            }
            else if (this._investmentStatusId == InvestmentStatus.Started.Id)
            {
                if (this.HasUnfinishedRoundtrip())
                {
                    if (forceClose)
                    {
                        this._investmentStatusId = InvestmentStatus.ForcedClosing.Id;

                        this.AddDomainEvent(new InvestmentForcedCloseDomainEvent(this));
                        return;
                    }
                    else
                    {
                        throw new InvestingDomainException("Could not close inventment when there is unfinished roundtrip, use force close instead.");
                    }
                }
            }
            else
            {
                throw new InvestingDomainException("Closing investment should happend after started and before close.");
            }


            this.EndBalance = this.CurrentBalance;
            this.CurrentBalance = 0;

            if (this._investmentTypeId != InvestmentType.Backtesting.Id)
            {
                this.DateClosed = DateTime.UtcNow;
            }

            this._investmentStatusId = InvestmentStatus.Closed.Id;

            this.AddDomainEvent(new InvestmentCloseDomainEvent(
                this
                ));
        }

        #endregion



        #region Private Functions
        private bool HasUnfinishedRoundtrip()
        {
            return this._investmentRoundtrips.Any(r => r.IsFinished() == false);
        }

        private void UpdateCurrentBalance(decimal currentBalance)
        {
            this.CurrentBalance = currentBalance;

            this.AddDomainEvent(new InvestmentBalanceChangedDomainEvent(
                this.InvestmentId,
                this.InvestmentType,
                this.Market,
                this.InitialBalance,
                this.CurrentBalance
                ));
        }
        #endregion
    }
}
