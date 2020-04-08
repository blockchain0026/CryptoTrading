using Autofac;
using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.Investing.API.Application.Commands;
using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using CryptoTrading.Services.Investing.Infrastructure.Idempotency;
using CryptoTrading.Services.Investing.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.API.Infrastructure.AutofacModules.ApplicationModule
{
    public class ApplicationModule : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;
        }

        protected override void Load(ContainerBuilder builder)
        {
            /*builder.Register(c => new OrderQueries(QueriesConnectionString))
                       .As<IOrderQueries>()
                       .InstancePerLifetimeScope();*/

            builder.RegisterType<FundRepository>()
                .As<IFundRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<InvestmentRepository>()
                .As<IInvestmentRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RoundtripRepository>()
                .As<IRoundtripRepository>()
                .InstancePerLifetimeScope();


            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(CreateInvestmentCommandHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

        }
    }
}
