using CryptoTrading.Services.Investing.API.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Validations
{
    public class SetPeriodForBacktestingCommandValidator : AbstractValidator<SetPeriodForBacktestingCommand>
    {
        public SetPeriodForBacktestingCommandValidator()
        {
            long start = 1199145600;
            long unixNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            RuleFor(command => command.InvestmentId).NotEmpty();
            RuleFor(command => command.From).NotEmpty().InclusiveBetween(start, unixNow);
            RuleFor(command => command.To).NotEmpty().InclusiveBetween(start, unixNow);
        }
    }
}
