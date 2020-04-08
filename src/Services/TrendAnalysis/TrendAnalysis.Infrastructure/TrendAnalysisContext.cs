using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Domain.SeedWork;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.EntityConfigurations;
using CryptoTrading.Services.TrendAnalysis.Infrastructure.Idempotency;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure
{
    public class TrendAnalysisContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "trendanalysis";
        public DbSet<Trace> Traces { get; set; }
        public DbSet<TradeStrategy> TradeStrategys { get; set; }
        public DbSet<TradeAdvice> TradeAdvice { get; set; }
        public DbSet<TraceStatus> TraceStatus { get; set; }
        public DbSet<TradingSignalType> TradingSignalType { get; set; }
        public DbSet<CandlePeriod> CandlePeriod { get; set; }

        public DbSet<ClientRequest> ClientRequests { get; set; }

        private readonly IMediator _mediator;

        private TrendAnalysisContext(DbContextOptions<TrendAnalysisContext> options) : base(options) { }

        public TrendAnalysisContext(DbContextOptions<TrendAnalysisContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("TrendAnalysisContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TraceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TradeStrategyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TradeAdviceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TraceStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TradingSignalTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CandlePeriodEntityTypeConfiguration());
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


    public class TrendAnalysisContextDesignFactory : IDesignTimeDbContextFactory<TrendAnalysisContext>
    {
        public TrendAnalysisContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public TrendAnalysisContext CreateDbContext(string[] args)
        {

            /*var optionsBuilder = new DbContextOptionsBuilder<ExecutionContext>()
                .UseSqlServer("Server=.;Initial Catalog=CryptoArbitrage.Services.ExecutionDb;Integrated Security=true");*/
            var optionsBuilder = new DbContextOptionsBuilder<TrendAnalysisContext>()
                .UseSqlServer("Server=sql.data;Database=cryptotrading.trendanalysis;User Id=sa;Password=1Secure*Password1;",
                sqlServerOptionsAction: sqlOptions =>
                {
                    var assemblyName = "TrendAnalysis.API";
                    sqlOptions.MigrationsAssembly(assemblyName);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            return new TrendAnalysisContext(optionsBuilder.Options, new NoMediator());
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
