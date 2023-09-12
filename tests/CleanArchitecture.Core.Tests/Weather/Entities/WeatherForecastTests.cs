using CleanArchitecture.Core.Abstractions.Exceptions;
using CleanArchitecture.Core.Tests.Builders;
using CleanArchitecture.Core.Weather.DomainEvents;
using CleanArchitecture.Core.Weather.Entities;
using CleanArchitecture.Core.Weather.ValueObjects;

namespace CleanArchitecture.Core.Tests.Weather.Entities
{
    public class WeatherForecastTests
    {
        [Fact]
        public void GivenWeatherForecast_WhenCreate_ThenCreate()
        {
            WeatherForecast forecast = new WeatherForecastBuilder().Build();
            forecast.Summary.Should().NotBeNullOrWhiteSpace();
            forecast.DomainEvents.Where(e => e is WeatherForecastCreatedDomainEvent).Should().HaveCount(1);
        }

        [Fact]
        public void GivenWeatherForecast_WhenTemperature10C_ThenCalculateTemperature50F()
        {
            WeatherForecast forecast = new WeatherForecastBuilder().WithTemperature(10).Build();
            int farenheit = forecast.Temperature.Farenheit;
            farenheit.Should().Be(50);
        }

        [Fact]
        public void GivenWeatherForecast_WhenTemperatureBelowAbsoluteZero_ThenError()
        {
            WeatherForecastBuilder forecastBuilder = new WeatherForecastBuilder().WithTemperature(-300);
            Action action = () => forecastBuilder.Build();
            action.Should().Throw<DomainException>().WithMessage("Temperature cannot be below Absolute Zero");
        }

        [Fact]
        public void GivenWeatherForecast_WhenSummaryEmpty_ThenError()
        {
            WeatherForecastBuilder forecastBuilder = new WeatherForecastBuilder().WithSummary(null);
            Action action = () => forecastBuilder.Build();
            action.Should().Throw<DomainException>().WithMessage("Required input 'Summary' is missing.");
        }

        [Fact]
        public void GivenWeatherForecast_WhenUpdate_ThenUpdate()
        {
            WeatherForecast forecast = new WeatherForecastBuilder().Build();
            forecast.Update(Temperature.FromCelcius(21), "Hot");
            forecast.Summary.Should().Be("Hot");
        }
    }
}