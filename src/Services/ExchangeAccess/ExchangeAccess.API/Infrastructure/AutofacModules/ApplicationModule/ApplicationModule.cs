using Autofac;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using CryptoTrading.Services.ExchangeAccess.Infrastructure.Idempotency;
using CryptoTrading.Services.ExchangeAccess.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.API.Infrastructure.AutofacModules.ApplicationModule
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

            builder.RegisterType<MarketRepository>()
                .As<IMarketRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BalanceRepository>()
                .As<IBalanceRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CandleChartRepository>()
                .As<ICandleChartRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();

            /*builder.RegisterAssemblyTypes(typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>)); */

        }
    }
}
