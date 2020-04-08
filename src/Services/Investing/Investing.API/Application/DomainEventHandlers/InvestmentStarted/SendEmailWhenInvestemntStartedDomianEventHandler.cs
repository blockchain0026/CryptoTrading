using CryptoTrading.Services.Investing.API.Services;
using CryptoTrading.Services.Investing.Domain.Events;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.InvestmentStarted
{
    public class SendEmailWhenInvestemntStartedDomianEventHandler : INotificationHandler<InvestmentStartedDomainEvent>
    {
        private readonly IEmailService _emailService;

        public SendEmailWhenInvestemntStartedDomianEventHandler(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Handle(InvestmentStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var investment = notification.Investment;


                var title = "炒幣狀態更新：開始新投資";
                string message = null;

                var dateStarted = (DateTime)investment.DateStarted;
                var twDateStarted = dateStarted.AddHours(8);

                if (investment.InvestmentType.Id == InvestmentType.Paper.Id)
                {
                    message =
                        "投資ID：" + investment.InvestmentId + " \n" +
                        "類別：實時模擬交易 \n" +
                        "模擬資金：" + investment.InitialBalance + investment.Market.QuoteCurrency + " \n" +
                        "操作市場：" + investment.Market.QuoteCurrency + " \n" +
                        "買賣幣種：" + investment.Market.BaseCurrency + " \n" +
                        "開始時間：" + twDateStarted.ToString();
                }
                else if (investment.InvestmentType.Id == InvestmentType.Live.Id)
                {
                    message = "投資ID：" + investment.InvestmentId + " \n" +
                        "類別：實時交易 \n" +
                        "初始資金：" + investment.InitialBalance + investment.Market.QuoteCurrency + " \n" +
                        "操作市場：" + investment.Market.QuoteCurrency + " \n" +
                        "買賣幣種：" + investment.Market.BaseCurrency + " \n" +
                        "開始時間：" + twDateStarted.ToString();
                }

                if (message != null)
                    await _emailService.Send(title, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: InvestmentStartedDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }
    }
}
