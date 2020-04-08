using CryptoTrading.Services.Investing.API.Services;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentBalanceChanged
{
    public class SendEmailWhenInvestmentBalanceChangedDomainEventHandler : INotificationHandler<InvestmentBalanceChangedDomainEvent>
    {
        private readonly IEmailService _emailService;

        public SendEmailWhenInvestmentBalanceChangedDomainEventHandler(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Handle(InvestmentBalanceChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                if (notification.InvestmentType.Id == InvestmentType.Backtesting.Id)
                {
                    return;
                }


                var title = "炒幣狀態更新：資金變動";
                string message = null;


                message =
                    "投資ID：" + notification.InvestmentId + " \n" +
                    "操作市場：" + notification.Market.QuoteCurrency + " \n" +
                    "買賣幣種：" + notification.Market.BaseCurrency + " \n" +
                    "初始資金：" + notification.InitialBalance.ToString() + notification.Market.QuoteCurrency + " \n" +
                    "當前資金：" + notification.CurrentBalance.ToString() + notification.Market.QuoteCurrency + " \n" +
                    "收益率：" + notification.CurrentBalance/ notification.InitialBalance;



                if (message != null)
                    await _emailService.Send(title, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentBalanceChangedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
