using CleanArchitecture.Application.Locations.Models;

namespace CleanArchitecture.Api.Tests.Controllers
{
    public class LocationsControllerTests
    {
        private const string BASE_URL = "api/locations";
        private readonly TestWebApplication _application = new();

        [Fact]
        public async Task GivenLocationsController_WhenGet_ThenOk()
        {
            using HttpClient client = _application.CreateClient();
            HttpResponseMessage response = await client.GetAsync(BASE_URL);

            List<LocationDto>? locations = await response.ReadAndAssertSuccessAsync<List<LocationDto>>();

            locations.Should().HaveCount(_application.TestLocations.Count);
        }
    }
}