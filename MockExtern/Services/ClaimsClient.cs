using Marvin.StreamExtensions;
using Shared;
using System.Net.Http.Headers;
using System.Text;

namespace MockExtern.Services
{
    public class ClaimsClient
    {
        private HttpClient httpClient;

        public ClaimsClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://localhost:7142");
            this.httpClient.Timeout = new TimeSpan(0, 0, 30);
            this.httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task<Claim> GetStatus(CancellationToken cancellationToken)
        {
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

                // TODO: Add errorhandling here
                response.EnsureSuccessStatusCode();

                return stream.ReadAndDeserializeFromJson<Claim>();
            }
        }

        public async Task<Claim> SubmitClaim()
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

                    // TODO: Here we need to att a switch to handle various error codes
                    // that we get back from Grundsystemets API
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();
                    return stream.ReadAndDeserializeFromJson<Claim>();
                }
            }
        }

        // This needs to be run separately from the SubmitClaim-method. One sends
        // a json, another sends a PDF, but both are needed.
        public async Task SubmitClaimPDF()
        {

            var fileRoute = "wwwroot/testPDFToSend/testClaim.pdf";
            var fileName = Path.GetFileName(fileRoute);

            using var formContent = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(fileRoute);
            formContent.Add(new StreamContent(fileStream), "file", fileName);

            var response = await httpClient.PostAsync("api/claims/postpdf", formContent);

            //TODO: add errorhandling for various error status codes...
            response.EnsureSuccessStatusCode();
        }
    }
}
