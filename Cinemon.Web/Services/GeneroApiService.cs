using Cinemon.Web.Models.DTOs;

namespace Cinemon.Web.Services
{
    public sealed class GeneroApiService
    {
        private readonly HttpClient _httpClient;

        public GeneroApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }

        public async Task<IReadOnlyCollection<GeneroDto>> ObtenerTodosAsync()
        {
            return await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<GeneroDto>>(
                    "api/generos") ?? [];
        }
    }
}
