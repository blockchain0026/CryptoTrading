using CryptoTrading.Services.Investing.API.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Application.Validations
{
    public class CreateInvestmentCommandValidator : AbstractValidator<CreateInvestmentCommand>
    {
        public CreateInvestmentCommandValidator()
        {
   
            RuleFor(command => command.InvestmentType).NotEmpty();

            //Don't use NotEmpty() method, it will throw exception if the value is 0.
            RuleFor(command => command.ExchangeId).GreaterThanOrEqualTo(0);

            RuleFor(command => command.BaseCurrency).NotEmpty();
            RuleFor(command => command.QuoteCurrency).NotEmpty();
            RuleFor(command => command.Username).NotEmpty();
        }
    }
}
