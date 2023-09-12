using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace CleanArchitecture.Infrastructure
{
    public sealed class WeatherContext : DbContext
    {
        private static readonly ILoggerFactory DebugLoggerFactory =
            new LoggerFactory(new[] { new DebugLoggerProvider() });

        private readonly IHostEnvironment? _env;

        public WeatherContext(DbContextOptions<WeatherContext> options,
            IHostEnvironment? env) : base(options)
        {
            _env = env;
        }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }

        public DbSet<Location> Locations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_env != null && _env.IsDevelopment())
            {
                // used to print activity when debugging
                optionsBuilder.UseLoggerFactory(DebugLoggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatherForecastConfiguration).Assembly);
            IEnumerable<Type> aggregateTypes = modelBuilder.Model
                .GetEntityTypes()
                .Select(e => e.ClrType)
                .Where(e => !e.IsAbstract && e.IsAssignableTo(typeof(AggregateRoot)));

            foreach (Type type in aggregateTypes)
            {
                EntityTypeBuilder aggregateBuild = modelBuilder.Entity(type);
                aggregateBuild.Ignore(nameof(AggregateRoot.DomainEvents));
                aggregateBuild.Property(nameof(AggregateRoot.Id)).ValueGeneratedNever();
            }
        }
    }
}