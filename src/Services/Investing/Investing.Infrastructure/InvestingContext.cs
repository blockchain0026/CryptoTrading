using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using CryptoTrading.Services.Investing.Domain.SeedWork;
using CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations;
using CryptoTrading.Services.Investing.Infrastructure.Idempotency;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Investing.Infrastructure
{
    public class InvestingContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "investing";

        public DbSet<Investment> Investments { get; set; }
        public DbSet<Roundtrip> Roundtrips { get; set; }
        public DbSet<Fund> Funds { get; set; }

        public DbSet<InvestmentRoundtrip> InvestmentRoundtrips { get; set; }
        public DbSet<InvestingFund> InvestingFunds { get; set; }

        public DbSet<InvestmentStatus> InvestmentStatus { get; set; }
        public DbSet<InvestmentType> InvestmentType { get; set; }
        public DbSet<RoundtripStatus> RoundtripStatus { get; set; }
        public DbSet<InvestingFundStatus> InvestingFundStatus { get; set; }


        public DbSet<ClientRequest> ClientRequests { get; set; }

        private readonly IMediator _mediator;

        private InvestingContext(DbContextOptions<InvestingContext> options) : base(options) { }

        public InvestingContext(DbContextOptions<InvestingContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("InvestingContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FundEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InvestingFundEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InvestingFundStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InvestmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InvestmentRoundtripEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InvestmentStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InvestmentTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoundtripEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoundtripStatusEntityTypeConfiguration());
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


    public class InvestingContextDesignFactory : IDesignTimeDbContextFactory<InvestingContext>
    {
        public InvestingContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public InvestingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InvestingContext>()
                .UseSqlServer("Server=sql.data;Database=cryptotrading.investing;User Id=sa;Password=1Secure*Password1;",
                sqlServerOptionsAction: sqlOptions =>
                {
                    var assemblyName = "Investing.API";
                    sqlOptions.MigrationsAssembly(assemblyName);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            return new InvestingContext(optionsBuilder.Options, new NoMediator());
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
