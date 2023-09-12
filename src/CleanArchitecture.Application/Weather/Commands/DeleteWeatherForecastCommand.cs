using CleanArchitecture.Application.Abstractions.Commands;
using CleanArchitecture.Application.Abstractions.Repositories;
using CleanArchitecture.Core.Abstractions.Guards;
using CleanArchitecture.Core.Weather.Entities;

namespace CleanArchitecture.Application.Weather.Commands
{
    public sealed record DeleteWeatherForecastCommand(Guid Id) : Command;

    public sealed class DeleteWeatherForecastCommandHandler : CommandHandler<DeleteWeatherForecastCommand>
    {
        private readonly IRepository<WeatherForecast> _repository;

        public DeleteWeatherForecastCommandHandler(IRepository<WeatherForecast> repository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
        }

        protected override async Task HandleAsync(DeleteWeatherForecastCommand request)
        {
            WeatherForecast? forecast = await _repository.GetByIdAsync(request.Id);
            forecast = Guard.Against.NotFound(forecast);
            _repository.Delete(forecast);
            await UnitOfWork.CommitAsync();
        }
    }
}