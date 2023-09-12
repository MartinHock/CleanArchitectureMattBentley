namespace CleanArchitecture.Api.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly TestWebApplication _application = new();

        [Fact]
        public async Task GivenHealthEndpoint_WhenHealthy_ThenOk()
        {
            using HttpClient client = _application.CreateClient();
            HttpResponseMessage response = await client.GetAsync("healthz");
            Assert.True(response.StatusCode == HttpStatusCode.OK ||
                        response.StatusCode == HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task GivenLivenessEndpoint_WhenHealthy_ThenOk()
        {
            using HttpClient client = _application.CreateClient();
            HttpResponseMessage response = await client.GetAsync("liveness");

            response.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}