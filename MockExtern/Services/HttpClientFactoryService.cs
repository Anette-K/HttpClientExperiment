using Marvin.StreamExtensions;
using Shared;
using System.Net.Http.Headers;

namespace MockExtern.Services
{
    public class HttpClientFactoryService : IIntegrationService
    {

        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ClaimsClient claimsClient;

        public HttpClientFactoryService(IHttpClientFactory httpClientFactory, ClaimsClient claimsClient)
        {
            this.httpClientFactory = httpClientFactory
                ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.claimsClient = claimsClient
                ?? throw new ArgumentNullException(nameof(claimsClient));
        }
        public async Task Run()
        {
            //await GetStatusWithNamedHttpClientFactory(cancellationSource.Token);
            //await GetStatusWithTypedClient(cancellationSource.Token);
            //await SubmitClaimWithTypedClient(cancellationSource.Token);
            await SubmitPDFWithHttpClientFactory();
        }

        private async Task SubmitClaimWithTypedClient(CancellationToken token)
        {
            var client = await claimsClient.SubmitClaim();
        }

        private async Task GetStatusWithTypedClient(CancellationToken cancellationToken)
        {
            var claim = await claimsClient.GetStatus(cancellationToken);
        }

        private async Task GetStatusWithNamedHttpClientFactory(CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("MockExternClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/claims/1");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var claim = stream.ReadAndDeserializeFromJson<Claim>();
            }


        }

        private async Task SubmitPDFWithHttpClientFactory()
        {
            await claimsClient.SubmitClaimPDF();
        }
    }
}
