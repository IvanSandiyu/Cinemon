using Cinemon.Web.Models.DTOs;
using System.Net.Http;

namespace Cinemon.Web.Services
{
    public sealed class PeliculaApiService
    {
        private readonly HttpClient _httpClient;

        public PeliculaApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }

        public async Task<IReadOnlyCollection<PeliculaDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            var peliculas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<PeliculaDto>>(
                    "api/peliculas",
                    cancellationToken);

            return peliculas ?? [];
        }

        public async Task<PeliculaDto?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<PeliculaDto>(
                $"api/peliculas/{id}",
                cancellationToken);
        }
    }
}
