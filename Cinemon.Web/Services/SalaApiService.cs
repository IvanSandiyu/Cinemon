using Cinemon.Web.Models.DTOs;

namespace Cinemon.Web.Services
{
    public sealed class SalaApiService
    {
        private readonly HttpClient _httpClient;

        public SalaApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }

        public async Task<IReadOnlyCollection<SalaDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            var salas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<SalaDto>>(
                    "api/salas",
                    cancellationToken);

            return salas ?? [];
        }
    }
}