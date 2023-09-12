using AutoMapper;
using CleanArchitecture.Application.Abstractions.Queries;
using CleanArchitecture.Application.Abstractions.Repositories;
using CleanArchitecture.Application.Weather.Models;
using CleanArchitecture.Core.Weather.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Weather.Queries
{
    public sealed record GetWeatherForecastsQuery(Guid? LocationId) : Query<List<WeatherForecastDto>>;

    public sealed class
        GetWeatherForecastsQueryHandler : QueryHandler<GetWeatherForecastsQuery, List<WeatherForecastDto>>
    {
        private readonly IRepository<WeatherForecast> _repository;

        public GetWeatherForecastsQueryHandler(IMapper mapper,
            IRepository<WeatherForecast> repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<List<WeatherForecastDto>> HandleAsync(GetWeatherForecastsQuery request)
        {
            IQueryable<WeatherForecast> forecastsQuery = _repository.GetAll();

            if (request.LocationId.HasValue)
            {
                forecastsQuery = forecastsQuery.Where(e => e.LocationId == request.LocationId.Value);
            }

            List<WeatherForecast> forecasts = await forecastsQuery.OrderBy(e => e.Date)
                .ToListAsync();

            return Mapper.Map<List<WeatherForecastDto>>(forecasts);
        }
    }
}