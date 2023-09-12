using CleanArchitecture.Application.Abstractions.Repositories;
using CleanArchitecture.Core.Tests.Builders;
using CleanArchitecture.Core.Weather.Entities;
using CleanArchitecture.Infrastructure.Tests.Repositories.Abstract;

namespace CleanArchitecture.Infrastructure.Tests.Repositories
{
    public class RepositoryTests : BaseRepositoryTests
    {
        [Fact]
        public async Task GivenRepository_WhenInsert_ThenInserted()
        {
            IRepository<WeatherForecast> repository = GetRepository<WeatherForecast>();
            WeatherForecast entity = new WeatherForecastBuilder().WithLocation(Location.Id).Build();

            repository.Insert(entity);
            await GetUnitOfWork().CommitAsync();

            WeatherForecast? inserted = await repository.GetByIdAsync(entity.Id);
            inserted.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenRepository_WhenInsertMultiple_ThenInserted()
        {
            IRepository<WeatherForecast> repository = GetRepository<WeatherForecast>();

            List<WeatherForecast> entities = new List<WeatherForecast>
            {
                new WeatherForecastBuilder().WithLocation(Location.Id).Build(),
                new WeatherForecastBuilder().WithLocation(Location.Id).Build()
            };

            repository.Insert(entities);
            await GetUnitOfWork().CommitAsync();

            WeatherForecast? inserted = await repository.GetByIdAsync(entities.Last().Id);
            inserted.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenRepository_WhenUpdate_ThenUpdated()
        {
            IRepository<WeatherForecast> repository = GetRepository<WeatherForecast>();
            DateTime date = DateTime.UtcNow;
            WeatherForecast entity = new WeatherForecastBuilder().WithLocation(Location.Id).Build();

            repository.Insert(entity);
            await GetUnitOfWork().CommitAsync();

            WeatherForecast? inserted = await repository.GetByIdAsync(entity.Id);
            inserted?.UpdateDate(date);

            await GetUnitOfWork().CommitAsync();

            WeatherForecast? updated = await repository.GetByIdAsync(entity.Id);
            updated!.Id.Should().Be(entity.Id);
            updated.Date.Should().Be(date);
        }

        [Fact]
        public async Task GivenRepository_WhenDelete_ThenDeleted()
        {
            IRepository<WeatherForecast> repository = GetRepository<WeatherForecast>();
            WeatherForecast entity = new WeatherForecastBuilder().WithLocation(Location.Id).Build();

            repository.Insert(entity);
            Guid id = entity.Id;
            await GetUnitOfWork().CommitAsync();

            repository.Delete(entity);
            await GetUnitOfWork().CommitAsync();
            WeatherForecast? inserted = await repository.GetByIdAsync(id);
            inserted.Should().BeNull();
        }

        [Fact]
        public async Task GivenRepository_WhenRemove_ThenRemoved()
        {
            IRepository<WeatherForecast> repository = GetRepository<WeatherForecast>();
            WeatherForecast entity = new WeatherForecastBuilder().WithLocation(Location.Id).Build();
            repository.Insert(entity);
            Guid id = entity.Id;
            await GetUnitOfWork().CommitAsync();

            repository.Remove(new List<WeatherForecast> { entity });
            await GetUnitOfWork().CommitAsync();
            WeatherForecast? inserted = await repository.GetByIdAsync(id);
            inserted.Should().BeNull();
        }

        [Fact]
        public async Task GivenRepository_WhenGetAll_ThenGetAll()
        {
            IRepository<WeatherForecast> repository = GetRepository<WeatherForecast>();
            WeatherForecast entity1 = new WeatherForecastBuilder().WithLocation(Location.Id).Build();
            WeatherForecast entity2 = new WeatherForecastBuilder().WithLocation(Location.Id).Build();
            repository.Insert(entity1);
            repository.Insert(entity2);
            await GetUnitOfWork().CommitAsync();

            IQueryable<WeatherForecast> inserted = repository.GetAll();
            inserted.ToList().Count.Should().Be(2);
        }

        [Fact]
        public async Task GivenRepository_WhenGetAllTracked_ThenChangesCommitted()
        {
            IRepository<WeatherForecast> repository = GetRepository<WeatherForecast>();
            WeatherForecast entity1 = new WeatherForecastBuilder().WithLocation(Location.Id).Build();
            WeatherForecast entity2 = new WeatherForecastBuilder().WithLocation(Location.Id).Build();
            DateTime date = DateTime.UtcNow;

            repository.Insert(entity1);
            Guid id1 = entity1.Id;
            repository.Insert(entity2);
            await GetUnitOfWork().CommitAsync();

            WeatherForecast? inserted = repository.GetAll(false).Where(u => u.Id == id1).FirstOrDefault();
            inserted?.UpdateDate(date);
            await GetUnitOfWork().CommitAsync();

            WeatherForecast? updated = await repository.GetByIdAsync(id1);
            updated!.Date.Should().Be(date);
        }
    }
}