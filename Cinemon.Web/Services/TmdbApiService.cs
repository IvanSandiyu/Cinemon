using Cinemon.Web.Models.DTOs;
using System.Net.Http;

namespace Cinemon.Web.Services
{
    public class TmdbApiService
    {
        private readonly HttpClient _httpClient;
        public TmdbApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }
        public async Task<IReadOnlyCollection<TmdbMovieDto>> BuscarAsync(string query)
        {
            var response = await _httpClient.GetAsync(
                $"api/tmdb/search?query={Uri.EscapeDataString(query)}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<
                IReadOnlyCollection<TmdbMovieDto>>() ?? [];
        }

        public async Task VincularAsync(int peliculaId, int tmdbId)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/peliculas/{peliculaId}/tmdb",
                new { tmdbId });

            response.EnsureSuccessStatusCode();
        }

        public async Task<TmdbMovieDto?> ObtenerDetalleAsync(int tmdbId)
        {
            var response = await _httpClient.GetAsync(
                $"api/tmdb/{tmdbId}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TmdbMovieDto>();
        }
    }
}
