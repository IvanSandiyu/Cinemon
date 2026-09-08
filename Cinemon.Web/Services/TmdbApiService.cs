using Cinemon.Web.Models.DTOs;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cinemon.Web.Services
{
    public class TmdbApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public TmdbApiService(
            IHttpClientFactory httpClientFactory,
            AuthService authService)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
            _authService = authService;
        }

        private void AdjuntarToken()
        {
            if (!_authService.EstaAutenticado)
                return;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _authService.Token);
        }

        public async Task<IReadOnlyCollection<TmdbMovieDto>> BuscarAsync(string query)
        {
            AdjuntarToken();

            var response = await _httpClient.GetAsync(
                $"api/tmdb/search?query={Uri.EscapeDataString(query)}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<
                IReadOnlyCollection<TmdbMovieDto>>() ?? [];
        }

        public async Task VincularAsync(int peliculaId, int tmdbId)
        {
            AdjuntarToken();

            var response = await _httpClient.PostAsJsonAsync(
                $"api/peliculas/{peliculaId}/tmdb",
                new { tmdbId });

            response.EnsureSuccessStatusCode();
        }

        public async Task<TmdbMovieDto?> ObtenerDetalleAsync(int tmdbId)
        {
            AdjuntarToken();

            var response = await _httpClient.GetAsync(
                $"api/tmdb/{tmdbId}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TmdbMovieDto>();
        }
    }
}
