using Cinemon.Web.Models.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Cinemon.Web.Services
{
    public sealed class PromocionApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public PromocionApiService(IHttpClientFactory httpClientFactory,AuthService authService)
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

        public async Task<IReadOnlyCollection<PromocionDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            return await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<PromocionDto>>(
                    "api/promociones",
                    cancellationToken) ?? [];
        }

        public async Task<(bool Ok, string? Error)> CrearAsync(
            CrearPromocionRequest request)
        {
            AdjuntarToken();

            var response = await _httpClient.PostAsJsonAsync(
                "api/promociones",
                request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Ok, string? Error)> ActualizarAsync(
            ActualizarPromocionRequest request)
        {
            AdjuntarToken();

            var response = await _httpClient.PutAsJsonAsync(
                $"api/promociones/{request.Id}",
                request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Ok, string? Error)> CambiarEstadoAsync(
            int id,
            bool activa)
        {
            AdjuntarToken();

            var response = await _httpClient.PatchAsJsonAsync(
                $"api/promociones/{id}/estado",
                new { activa });

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Ok, string? Error)> EliminarAsync(int id)
        {
            AdjuntarToken();

            var response = await _httpClient.DeleteAsync($"api/promociones/{id}");

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }
    }
}
