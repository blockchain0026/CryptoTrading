using CryptoTrading.Services.Investing.API.Services;
using CryptoTrading.Services.Investing.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.RoundtripExit
{
    public class SendEmailWhenRoundtripExitDomainEventHandler : INotificationHandler<RoundtripExitDomainEvent>
    {
        private readonly IEmailService _emailService;

        public SendEmailWhenRoundtripExitDomainEventHandler(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Handle(RoundtripExitDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var roundtrip = notification.Roundtrip;


                var title = "炒幣狀態更新：賣出";
                string message = null;

                var exitAt = (DateTime)roundtrip.ExitAt;

                message =
                    "投資ID：" + roundtrip.InvestmentId + " \n" +
                    "操作市場：" + roundtrip.Market.QuoteCurrency + " \n" +
                    "買賣幣種：" + roundtrip.Market.BaseCurrency + " \n" +
                    "賣出時間：" + exitAt.AddHours(8).ToString() + " \n" +
                    "賣出價格：" + roundtrip.Transaction.SellPrice + " \n" +
                    "賣出數量：" + roundtrip.Transaction.SellAmount;


                if (message != null)
                    await _emailService.Send(title, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: RoundtripExitDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }

 
    }
}
