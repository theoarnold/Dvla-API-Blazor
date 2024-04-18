using System.Text.Json;
namespace Dvla.Services
{

    public class DvlaService  : IDvlaService
    {
        private readonly IHttpClientFactory _clientFactory;

        public DvlaService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IEnumerable<DvlaRes?>> GetGitHubBranchesAsync(string registration)
        {
            /* Requirements say it code must compile and run in one step.
               API key was stored in env variable as to not expose it on github.
               Leaving it blank here, to be replaced with exisitng key when testing.*/
            string apiKey = "PUT_API_KEY_HERE";
            string apiUrl = "https://beta.check-mot.service.gov.uk/trade/vehicles/mot-tests?registration=" + registration;
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("x-api-key", apiKey);

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

           if (response.IsSuccessStatusCode)
           {
               using var responseStream = await response.Content.ReadAsStreamAsync();
               return await JsonSerializer.DeserializeAsync<IEnumerable<DvlaRes>>(responseStream);
           }
           else
           {
                return null;
                throw new HttpRequestException($"Error fetching MOT tests for registration number {registration}. Status code: {response.StatusCode}");
           }
        }
    }

}
