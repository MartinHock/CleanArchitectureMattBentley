using CleanArchitecture.Core.Weather.Entities;
using CleanArchitecture.Core.Weather.ValueObjects;

namespace CleanArchitecture.Core.Tests.Builders
{
    public class WeatherForecastBuilder
    {
        private DateTime _date = DateTime.UtcNow;
        private Guid _location = new("B0C91847-8931-4C45-9FD5-018A3A3398CF");
        private string? _summary = "Mild";
        private int _temperature = 8;

        public WeatherForecast Build()
        {
            return WeatherForecast.Create(_date, Temperature.FromCelcius(_temperature), _summary, _location);
        }

        public WeatherForecastBuilder WithTemperature(int temperature)
        {
            _temperature = temperature;
            return this;
        }

        public WeatherForecastBuilder WithSummary(string? summary)
        {
            _summary = summary;
            return this;
        }

        public WeatherForecastBuilder WithDate(DateTime date)
        {
            _date = date;
            return this;
        }

        public WeatherForecastBuilder WithLocation(Guid locationId)
        {
            _location = locationId;
            return this;
        }
    }
}