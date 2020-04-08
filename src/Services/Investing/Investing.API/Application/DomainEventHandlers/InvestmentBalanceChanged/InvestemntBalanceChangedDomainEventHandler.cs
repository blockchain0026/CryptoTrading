using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentBalanceChanged
{
    public class InvestemntBalanceChangedDomainEventHandler : INotificationHandler<InvestmentBalanceChangedDomainEvent>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IFundRepository _fundRepository;

        public InvestemntBalanceChangedDomainEventHandler(IInvestmentRepository investmentRepository, IFundRepository fundRepository)
        {
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _fundRepository = fundRepository ?? throw new ArgumentNullException(nameof(fundRepository));
        }

        public async Task Handle(InvestmentBalanceChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var investment = await _investmentRepository.GetByInvestmentId(notification.InvestmentId);
                if (investment == null)
                {
                    throw new KeyNotFoundException(nameof(notification.InvestmentId));
                }

                var fund = await _fundRepository.GetBySymbol(investment.Market.QuoteCurrency.ToUpper());
                if (fund == null)
                {
                    throw new KeyNotFoundException(nameof(investment.Market.QuoteCurrency));
                }

                var matchFund = fund.Where(f => f.Account.Equals(investment.Account)).SingleOrDefault();
                if (matchFund == null)
                {
                    return;
                }

                matchFund.InvestmentFundUpdated(investment);

                this._fundRepository.Update(matchFund);

                await this._fundRepository.UnitOfWork.SaveEntitiesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentBalanceChangedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
