using System.Text.Json;

namespace Dvla.Services
{
    public class DvlaService : IDvlaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public DvlaService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _apiKey = configuration["DvlaApi:Key"] ?? throw new ArgumentNullException("DvlaApi:Key not configured");
            _baseUrl = configuration["DvlaApi:BaseUrl"] ?? "https://beta.check-mot.service.gov.uk/trade/vehicles/mot-tests";
        }

        public async Task<IEnumerable<DvlaRes>?> GetDvlaAsync(string registration)
        {
            if (string.IsNullOrWhiteSpace(registration))
            {
                throw new ArgumentException("Registration number cannot be empty", nameof(registration));
            }

            string escapedRegistration = Uri.EscapeDataString(registration);
            string apiUrl = $"{_baseUrl}?registration={escapedRegistration}";

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("x-api-key", _apiKey);

            HttpClient client = _clientFactory.CreateClient();

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using Stream responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<IEnumerable<DvlaRes>>(responseStream);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(
                    $"Error fetching MOT tests for registration number {registration}. Status code: {ex.StatusCode}",
                    ex);
            }
        }
    }
}