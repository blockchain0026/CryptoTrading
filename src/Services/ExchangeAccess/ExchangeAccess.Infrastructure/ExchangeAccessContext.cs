using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure
{
    public class ExchangeAccessContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "exchangeaccess";
        public DbSet<Market> Markets { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<CandleChart> CandleCharts { get; set; }
        public DbSet<CandlePeriod> CandlePeriod { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Candle> Candles { get; set; }
        public DbSet<MarketAsk> MarketAsks { get; set; }
        public DbSet<MarketBid> MarketBids { get; set; }


        private readonly IMediator _mediator;

        private ExchangeAccessContext(DbContextOptions<ExchangeAccessContext> options) : base(options) { }

        public ExchangeAccessContext(DbContextOptions<ExchangeAccessContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("ExchangeAccessContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MarketEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BalanceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CandleChartEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CandlePeriodEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AssetEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MarketAskEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MarketBidEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CandleEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 


            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed throught the DbContext will be commited
            var result = await base.SaveChangesAsync();

            await _mediator.DispatchDomainEventsAsync(this);

            return true;
        }

    }


    public class ExchangeAccessContextDesignFactory : IDesignTimeDbContextFactory<ExchangeAccessContext>
    {
        public ExchangeAccessContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public ExchangeAccessContext CreateDbContext(string[] args)
        {

            /*var optionsBuilder = new DbContextOptionsBuilder<ExecutionContext>()
                .UseSqlServer("Server=.;Initial Catalog=CryptoArbitrage.Services.ExecutionDb;Integrated Security=true");*/
            var optionsBuilder = new DbContextOptionsBuilder<ExchangeAccessContext>()
                .UseSqlServer("Server=sql.data;Database=cryptotrading.exchangeaccess;User Id=sa;Password=1Secure*Password1;",
                sqlServerOptionsAction: sqlOptions =>
                {
                    var assemblyName = "ExchangeAccess.API";
                    sqlOptions.MigrationsAssembly(assemblyName);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            return new ExchangeAccessContext(optionsBuilder.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult<TResponse>(default(TResponse));
            }

            public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.CompletedTask;
            }
        }
    }
}
