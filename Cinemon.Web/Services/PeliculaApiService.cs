using Cinemon.Web.Models.DTOs;
using Cinemon.Web.Models.DTOs.Requests;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cinemon.Web.Services
{
    public sealed class PeliculaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public PeliculaApiService(
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

        public async Task<IReadOnlyCollection<PeliculaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
        {
            var peliculas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<PeliculaDto>>(
                    "api/peliculas",
                    cancellationToken);

            return peliculas ?? [];
        }

        public async Task<PeliculaDto?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<PeliculaDto>(
                $"api/peliculas/{id}",
                cancellationToken);
        }

        public async Task ActivarAsync(int id)
        {
            AdjuntarToken();

            var response = await _httpClient.PatchAsync(
                $"api/peliculas/{id}/activar",
                null);

            response.EnsureSuccessStatusCode();
        }

        public async Task DesactivarAsync(int id)
        {
            AdjuntarToken();

            var response = await _httpClient.PatchAsync(
                $"api/peliculas/{id}/desactivar",
                null);

            response.EnsureSuccessStatusCode();
        }

        public async Task<(int Id, string? Error)> CrearAsync(CrearPeliculaRequest request)
        {
            AdjuntarToken();

            var response = await _httpClient.PostAsJsonAsync(
                "api/peliculas",
                request);

            if (!response.IsSuccessStatusCode) {
                var contenido = await response.Content
                    .ReadAsStringAsync();

                return (0, contenido);
            }

            var peliculaId = await response.Content.ReadFromJsonAsync<int>();

            return (peliculaId, null);
        }


    }
}
