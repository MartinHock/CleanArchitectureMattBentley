using CleanArchitecture.Application.Abstractions.Commands;
using CleanArchitecture.Application.Abstractions.Repositories;
using CleanArchitecture.Core.Abstractions.Guards;
using CleanArchitecture.Core.Weather.Entities;

namespace CleanArchitecture.Application.Weather.Commands
{
    public sealed record UpdateWeatherForecastCommand(Guid Id, DateTime Date) : Command;

    public sealed class UpdateWeatherForecastCommandHandler : CommandHandler<UpdateWeatherForecastCommand>
    {
        private readonly IRepository<WeatherForecast> _repository;

        public UpdateWeatherForecastCommandHandler(IRepository<WeatherForecast> repository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
        }

        protected override async Task HandleAsync(UpdateWeatherForecastCommand request)
        {
            WeatherForecast? forecast = await _repository.GetByIdAsync(request.Id);
            forecast = Guard.Against.NotFound(forecast);
            forecast.UpdateDate(request.Date);
            await UnitOfWork.CommitAsync();
        }
    }
}