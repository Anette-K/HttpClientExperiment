using Marvin.StreamExtensions;
using Shared;
using System.Net;
using System.Net.Http.Headers;

namespace MockExtern.Services
{
    public class CancellationService : IIntegrationService
    {
        private static HttpClient httpClient = new HttpClient(
            new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip,
            });
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public CancellationService()
        {
            httpClient.BaseAddress = new Uri("https://localhost:7142");
            httpClient.Timeout = new TimeSpan(0, 0, 2);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task Run()
        {
            cancellationTokenSource.CancelAfter(1000);
            //await GetStatusAndCancel(cancellationTokenSource.Token);
            await GetStatusAndHandleTimeout();
        }

        private async Task GetStatusAndCancel(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/claims");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                using (var response = await httpClient.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead,
                                cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    response.EnsureSuccessStatusCode();
                    var claim = stream.ReadAndDeserializeFromJson<Claim>();
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"An operation was cancelled with message {e.Message}.");
                // additional cleanup or rerouting...
            }

        }

        private async Task GetStatusAndHandleTimeout()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/claims");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                using (var response = await httpClient.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    response.EnsureSuccessStatusCode();
                    var claim = stream.ReadAndDeserializeFromJson<Claim>();
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"An operation was cancelled with message {e.Message}.");
                // additional cleanup or rerouting...
            }
        }
    }
}
