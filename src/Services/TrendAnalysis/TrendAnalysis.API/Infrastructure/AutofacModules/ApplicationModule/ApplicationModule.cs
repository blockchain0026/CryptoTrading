using Autofac;
using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.Services.TrendAnalysis.API.Application.Commands;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.Idempotency;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Infrastructure.AutofacModules.ApplicationModule
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

            builder.RegisterType<TraceRepository>()
                .As<ITraceRepository>()
                .InstancePerLifetimeScope();

        
            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(CreateTraceCommandHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

        }
    }
}
