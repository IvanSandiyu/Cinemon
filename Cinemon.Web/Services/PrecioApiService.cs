using Cinemon.Web.Models.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Cinemon.Web.Services
{
    public sealed class PrecioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public PrecioApiService(IHttpClientFactory httpClientFactory,AuthService authService)
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

        public async Task<IReadOnlyCollection<PrecioDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<PrecioDto>>(
                    "api/precios",
                    cancellationToken) ?? [];
        }

        public async Task<(bool Ok, string? Error)> ActualizarAsync(
            int id,decimal valor)
        {
            AdjuntarToken();

            var response = await _httpClient.PutAsJsonAsync(
                $"api/precios/{id}",
                new { valor });

            if (response.IsSuccessStatusCode)
                return (true, null);

            var contenido = await response.Content
                .ReadAsStringAsync();

            return (false, contenido);
        }
    }
}
