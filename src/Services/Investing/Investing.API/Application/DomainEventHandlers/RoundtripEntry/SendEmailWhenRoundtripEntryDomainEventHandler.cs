using CryptoTrading.Services.Investing.API.Services;
using CryptoTrading.Services.Investing.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.DomainEventHandlers.RoundtripEntry
{
    public class SendEmailWhenRoundtripEntryDomainEventHandler : INotificationHandler<RoundtripEntryDomainEvent>
    {
        private readonly IEmailService _emailService;

        public SendEmailWhenRoundtripEntryDomainEventHandler(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Handle(RoundtripEntryDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {



                var title = "炒幣狀態更新：買入";
                string message = null;


                message =
                    "投資ID：" + notification.InvestmentId + " \n" +
                    "操作市場：" + notification.Market.QuoteCurrency + " \n" +
                    "買賣幣種：" + notification.Market.BaseCurrency + " \n" +
                    "買入時間：" + notification.EntryAt.AddHours(8).ToString() + " \n" +
                    "買入價格：" + notification.Transaction.BuyPrice + " \n" +
                    "買入數量：" + notification.Transaction.BuyAmount;


                if (message != null)
                    await _emailService.Send(title, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle Domain Event: RoundtripEntryDomainEvent.");
                Console.WriteLine("Result: Failure.");
                Console.WriteLine("Error Message: " + ex.Message);
            }
        }


    }
}
