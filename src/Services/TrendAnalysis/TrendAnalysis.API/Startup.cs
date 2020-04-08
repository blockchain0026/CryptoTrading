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
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.EventHandling;
using CryptoTrading.Services.TrendAnalysis.API.Application.IntegrationEvents.Events;
using CryptoTrading.Services.TrendAnalysis.API.Infrastructure;
using CryptoTrading.Services.TrendAnalysis.API.Infrastructure.AutofacModules.ApplicationModule;
using CryptoTrading.Services.TrendAnalysis.API.Infrastructure.AutofacModules.MediatorModule;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Indicators;
using CryptoTrading.Services.TrendAnalysis.Infrastructure;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.DomainServices;
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
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Swagger;

namespace TrendAnalysis.API
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

            services.AddTransient<ITrendAnalysisIntegrationEventService, TrendAnalysisIntegrationEventService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("cryptotrading.trendanalysis", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            });

            services.AddEntityFrameworkSqlServer()
             .AddDbContext<TrendAnalysisContext>(options =>
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


            services.AddTransient<IIndicatorService, IndicatorService>();


            //services.AddSingleton<IHostedService, ExchangeAccessHostService>();
            //services.AddSingleton<IExchangeAccessWebsocketService, ExchangeAccessWebsocketService>();



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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api/docs";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trend Analysis API V1");
            });


            ConfigureEventBus(app);

            TrendAnalysisContextSeed.SimpleSeedAsync(app).Wait();
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
            services.AddTransient<BacktestingDataCreatedIntegrationEventHandler>();
            services.AddTransient<InvestmentSettledIntegrationEventHandler>();
            services.AddTransient<BacktestingInvestmentStartedIntegrationEventHandler>();
            services.AddTransient<PaperInvestmentStartedIntegrationEventHandler>();
            services.AddTransient<InvestmentCandleDataRequestedIntegrationEventHandler>();
            services.AddTransient<PaperTradeDataCreatedIntegrationEventHandler>();
            services.AddTransient<RoundtripTargetPriceHitIntegrationEventHandler>();
            services.AddTransient<TargetPriceCandleDataCreatedIntegrationEventHandler>();
            services.AddTransient<LiveInvestmentStartedIntegrationEventHandler>();
            #endregion
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            #region Integration Events
            eventBus.Subscribe
                <BacktestingDataCreatedIntegrationEvent,
                BacktestingDataCreatedIntegrationEventHandler>();
            eventBus.Subscribe
                <InvestmentSettledIntegrationEvent,
                InvestmentSettledIntegrationEventHandler>();
            eventBus.Subscribe
                <BacktestingInvestmentStartedIntegrationEvent,
                BacktestingInvestmentStartedIntegrationEventHandler>();
            eventBus.Subscribe
                <PaperInvestmentStartedIntegrationEvent,
                PaperInvestmentStartedIntegrationEventHandler>();
            eventBus.Subscribe
                <InvestmentCandleDataRequestedIntegrationEvent,
                InvestmentCandleDataRequestedIntegrationEventHandler>();
            eventBus.Subscribe
                <PaperTradeDataCreatedIntegrationEvent,
                PaperTradeDataCreatedIntegrationEventHandler>();
            eventBus.Subscribe
                <RoundtripTargetPriceHitIntegrationEvent,
                RoundtripTargetPriceHitIntegrationEventHandler>();
            eventBus.Subscribe
                <TargetPriceCandleDataCreatedIntegrationEvent,
                TargetPriceCandleDataCreatedIntegrationEventHandler>();
            eventBus.Subscribe
                <LiveInvestmentStartedIntegrationEvent,
                LiveInvestmentStartedIntegrationEventHandler>();
            #endregion
        }
    }
}
