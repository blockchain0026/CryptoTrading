using CryptoTrading.Services.TrendAnalysis.API.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.Validations
{
    public class CreateTraceCommandValidator : AbstractValidator<CreateTraceCommand>
    {
        public CreateTraceCommandValidator()
        {
            //long start = 1199145600;
            //long unixNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            RuleFor(command => command.InvestmentId).NotEmpty();
            //Don't use NotEmpty() method, it will throw exception if the value is 0.
            RuleFor(command => command.ExchangeId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.BaseCurrency).NotEmpty();
            RuleFor(command => command.QuoteCurrency).NotEmpty();
            
            /*RuleFor(command => command.From).NotEmpty().InclusiveBetween(start, unixNow);
            RuleFor(command => command.To).NotEmpty().InclusiveBetween(start,unixNow);*/

        }
    }
}
