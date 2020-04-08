using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CryptoTrading.BuildingBlocks.EventBus;
using CryptoTrading.BuildingBlocks.EventBus.Abstractions;
using CryptoTrading.BuildingBlocks.EventBus.EventBusRabbitMQ;
using CryptoTrading.BuildingBlocks.EventBus.IntegrationEventLogEF.Services;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.EventHandling;
using CryptoTrading.Services.ExchangeAccess.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.ExchangeAccess.API.Infrastructure;
using CryptoTrading.Services.ExchangeAccess.API.Infrastructure.AutofacModules.ApplicationModule;
using CryptoTrading.Services.ExchangeAccess.API.Infrastructure.AutofacModules.MediatorModule;
using CryptoTrading.Services.ExchangeAccess.API.Services;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using CryptoTrading.Services.ExchangeAccess.Infrastructure;
using CryptoTrading.Services.ExchangeAccess.Infrastructure.DomainServices;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Swagger;

namespace ExchangeAccess.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IExchangeAccessIntegrationEventService, ExchangeAccessIntegrationEventService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("cryptotrading.exchangeaccess", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            });

            services.AddEntityFrameworkSqlServer()
             .AddDbContext<ExchangeAccessContext>(options =>
             {
                 options.UseSqlServer(Configuration["ConnectionString"],
                     sqlServerOptionsAction: sqlOptions =>
                     {
                         var assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                         sqlOptions.MigrationsAssembly(assemblyName);
                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         sqlOptions.CommandTimeout(3 * 60);

                     });
             },
                 ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
             );

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Crypto Trading Exchanges Access API Document",
                    Version = "v1",
                    Description = "",
                    TermsOfService = "None"
                });
                /*var xmlFile = "ExchangesAccess.API.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);*/
            });


            //services.Configure<ExecutionSettings>(Configuration);


            services.AddTransient<IExchangeAccessService, ExchangeAccessService>();


            services.AddSingleton<IHostedService, ExchangeAccessHostService>();
            services.AddSingleton<IExchangeAccessWebsocketService, ExchangeAccessWebsocketService>();



            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                //var settings = sp.GetRequiredService<IOptions<CatalogSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }
                else
                {
                    factory.UserName = "guest";
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }
                else
                {
                    factory.Password = "guest";
                }
                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });




            //Configure event bus.
            RegisterEventBus(services);

            var container = new ContainerBuilder();
            container.Populate(services);

            services.AddMediatR();

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            /*app.UseSwaggerUi3WithApiExplorer(s =>
            {
                s.GeneratorSettings.DefaultPropertyNameHandling =
                PropertyNameHandling.CamelCase;
            });*/

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api/docs";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exchanges Access API V1");
            });


            ConfigureEventBus(app);

            ExchangeAccessContextSeed.SimpleSeedAsync(app).Wait();
        }


        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });


            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();


            #region Integration Events Handlers
            services.AddTransient<ExchangeOrderBookDataReceivedIntegrationEventHandler>();
            services.AddTransient<ExchangeCandleDataReceivedIntegrationEventHandler>();
            services.AddTransient<ExchangeBalanceDataReceivedIntegrationEventHandler>();
            services.AddTransient<BacktesingTraceStartedIntegrationEventHandler>();
            services.AddTransient<TraceDataRequestedIntegrationEventHandler>();
            services.AddTransient<TargetPriceCandleDataRequestedIntegrationEventHandler>();
            services.AddTransient<LiveRoundtripEntryOrderSubmittedIntegrationEventHandler>();
            services.AddTransient<LiveRoundtripExitOrderSubmittedIntegrationEventHandler>();
            #endregion
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            #region Integration Events
            eventBus.Subscribe
                <ExchangeOrderBookDataReceivedIntegrationEvent,
                ExchangeOrderBookDataReceivedIntegrationEventHandler>();
            eventBus.Subscribe
                <ExchangeCandleDataReceivedIntegrationEvent,
                ExchangeCandleDataReceivedIntegrationEventHandler>();
            eventBus.Subscribe
                <ExchangeBalanceDataReceivedIntegrationEvent,
                ExchangeBalanceDataReceivedIntegrationEventHandler>();
            eventBus.Subscribe
                <BacktestingTraceStartedIntegrationEvent,
                BacktesingTraceStartedIntegrationEventHandler>();
            eventBus.Subscribe
                <TraceDataRequestedIntegrationEvent,
                TraceDataRequestedIntegrationEventHandler>();
            eventBus.Subscribe
                <TargetPriceCandleDataRequestedIntegrationEvent,
                TargetPriceCandleDataRequestedIntegrationEventHandler>();
            eventBus.Subscribe
                <LiveRoundtripEntryOrderSubmittedIntegrationEvent,
                LiveRoundtripEntryOrderSubmittedIntegrationEventHandler>();
            eventBus.Subscribe
                <LiveRoundtripExitOrderSubmittedIntegrationEvent,
                LiveRoundtripExitOrderSubmittedIntegrationEventHandler>();
            #endregion
        }
    }
}
