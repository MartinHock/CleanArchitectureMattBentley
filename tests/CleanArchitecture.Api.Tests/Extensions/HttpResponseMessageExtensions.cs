using CleanArchitecture.Api.Infrastructure.ActionResults;
using Newtonsoft.Json;

namespace CleanArchitecture.Api.Tests
{
    internal static class HttpResponseMessageExtensions
    {
        public static async Task<T?> ReadAndAssertSuccessAsync<T>(this HttpResponseMessage response) where T : class
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            string json = await response.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
            {
                return json as T;
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<Envelope> ReadAndAssertError(this HttpResponseMessage response,
            HttpStatusCode statusCode)
        {
            response.StatusCode.Should().Be(statusCode);
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Envelope>(json)!;
        }
    }
}