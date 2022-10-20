using Shared;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MockExtern.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient httpClient = new HttpClient();

        public CRUDService()
        {
            httpClient.BaseAddress = new Uri("https://localhost:7142");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );

        }


        public async Task Run()
        {
            //await GetStatusCodeThroughHttpRequestMessage(1);
            await SubmitClaim();
        }


        public async Task GetStatusCodeThroughHttpRequestMessage(int id)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/claims/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var status = JsonSerializer.Deserialize<Claim>(content,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }

        public async Task SubmitClaim()
        {
            var claim = new Claim
            {
                FirstName = "Test2",
                LastName = "Testsson2",
                Income = 10
            };

            var serializedClaim = JsonSerializer.Serialize(claim);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/claims");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedClaim);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var postedClaim = JsonSerializer.Deserialize<Claim>(content,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }

        public async Task SubmitClaimShortcut()
        {
            var claim = new Claim
            {
                FirstName = "Test2",
                LastName = "Testsson2",
                Income = 10
            };

            var response = await httpClient.PostAsync(
                "api/claims",
                new StringContent(
                    JsonSerializer.Serialize(claim),
                    Encoding.UTF8,
                    "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var postedClaim = JsonSerializer.Deserialize<Claim>(content,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }

    }
}
