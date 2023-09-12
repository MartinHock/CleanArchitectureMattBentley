using CleanArchitecture.Core.Tests.Builders;

namespace CleanArchitecture.Api.Tests.Controllers
{
    public class ErrorsControllerTests
    {
        private const string BASE_URL = "api/weatherforecasts";
        private readonly TestWebApplication _application = new();

        public ErrorsControllerTests()
        {
            _application.TestWeatherForecasts.Add(new WeatherForecastBuilder().Build());
        }

        [Fact]
        public async Task GivenController_WhenUnhandledError_ThenInternalServerError()
        {
            using HttpClient client = _application.CreateClient();
            _application.WeatherForecastsRepository.Setup(e => e.GetByIdAsync(It.IsAny<Guid>()))
                .Throws(new Exception("There was an error"));

            HttpResponseMessage response =
                await client.GetAsync($"{BASE_URL}/{_application.TestWeatherForecasts.First().Id}");

            await response.ReadAndAssertError(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task GivenController_WhenUnauthorizedAccessException_ThenForbidden()
        {
            using HttpClient client = _application.CreateClient();
            _application.WeatherForecastsRepository.Setup(e => e.GetByIdAsync(It.IsAny<Guid>()))
                .Throws(new UnauthorizedAccessException("Unauthorized"));

            HttpResponseMessage response =
                await client.GetAsync($"{BASE_URL}/{_application.TestWeatherForecasts.First().Id}");

            await response.ReadAndAssertError(HttpStatusCode.Forbidden);
        }
    }
}