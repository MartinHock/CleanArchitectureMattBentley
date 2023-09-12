using Autofac;
using CleanArchitecture.Application.Abstractions.Repositories;
using CleanArchitecture.Core.Abstractions.Entities;
using CleanArchitecture.Core.Locations.Entities;
using CleanArchitecture.Core.Tests.Builders;
using CleanArchitecture.Infrastructure.AutofacModules;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CleanArchitecture.Infrastructure.Tests.Repositories.Abstract
{
    public abstract class BaseRepositoryTests : IAsyncLifetime
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        private readonly IContainer _container;
        protected readonly WeatherContext Database;
        protected readonly Location Location = new LocationBuilder().Build();

        public BaseRepositoryTests()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            DbContextOptions<WeatherContext> options = new DbContextOptionsBuilder<WeatherContext>()
                .UseSqlite(_connection)
                .Options;

            IConfigurationRoot configuration = new ConfigurationBuilder().Build();
            ContainerBuilder containerBuilder = new ContainerBuilder();

            IHostEnvironment env = Mock.Of<IHostEnvironment>();
            containerBuilder.RegisterInstance(env);
            containerBuilder.RegisterInstance(Mock.Of<IMediator>());
            Database = new WeatherContext(options, env);
            Database.Database.EnsureCreated();

            containerBuilder.RegisterModule(new InfrastructureModule(options, configuration));
            _container = containerBuilder.Build();
        }

        public async Task InitializeAsync()
        {
            IRepository<Location> locationsRepository = GetRepository<Location>();
            locationsRepository.Insert(Location);
            await GetUnitOfWork().CommitAsync();
        }

        public Task DisposeAsync()
        {
            Database.Dispose();
            _connection.Close();
            _connection.Dispose();
            return Task.CompletedTask;
        }

        protected IRepository<T> GetRepository<T>()
            where T : AggregateRoot
        {
            return _container.Resolve<IRepository<T>>();
        }

        protected IUnitOfWork GetUnitOfWork()
        {
            return _container.Resolve<IUnitOfWork>();
        }
    }
}