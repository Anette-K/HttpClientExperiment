using Marvin.StreamExtensions;
//using MockExtern.Extensions;
using Newtonsoft.Json;
using Shared;
using System.Net.Http.Headers;
using System.Text;

namespace MockExtern.Services
{
    public class StreamService : IIntegrationService
    {
        private static HttpClient httpClient = new HttpClient();

        public StreamService()
        {
            httpClient.BaseAddress = new Uri("https://localhost:7142");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task Run()
        {
            //await GetStatus(1);
            //await SubmitClaimAsStream();
            await SubmitAndReadClaimAsStream();
        }

        private async Task GetStatus(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/claims/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // This option starts the stream as soon as the header is read
            var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead);



            // Wrapped in using-statements since streams need to be disposed of
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                response.EnsureSuccessStatusCode();

                var claim = stream.ReadAndDeserializeFromJson<Claim>();

            }
        }

        private async Task SubmitClaimAsStream()
        {
            var testClaim = new Claim
            {
                FirstName = "Test2",
                LastName = "Testsson2",
                Income = 10
            };

            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(testClaim);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/claims"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var streamContent = new StreamContent(memoryContentStream))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var createdContent = await response.Content.ReadAsStringAsync();
                    var createdClaim = JsonConvert.DeserializeObject<Claim>(createdContent);
                }
            }

        }

        private async Task SubmitAndReadClaimAsStream()
        {
            var testClaim = new Claim
            {
                FirstName = "Test2",
                LastName = "Testsson2",
                Income = 10
            };

            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(testClaim, new UTF8Encoding(), 1024, true);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/claims"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var streamContent = new StreamContent(memoryContentStream))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    var response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();
                    var createdClaim = stream.ReadAndDeserializeFromJson<Claim>();
                }
            }

        }
    }


}
