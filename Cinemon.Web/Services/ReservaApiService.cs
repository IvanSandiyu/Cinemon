using Cinemon.Web.Models.DTOs;
using Cinemon.Web.Models.DTOs.Requests;
using System.Net.Http.Headers;

namespace Cinemon.Web.Services
{
    public sealed class ReservaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public ReservaApiService(
            IHttpClientFactory httpClientFactory,
            AuthService authService)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
            _authService = authService;
        }

        public async Task<HttpResponseMessage> CrearAsync(CrearReservaRequest request,CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            return await _httpClient.PostAsJsonAsync(
                "api/reservas",
                request,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<ReservaDto>> ObtenerMisReservasAsync(
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            var reservas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<ReservaDto>>(
                    "api/reservas/mis-reservas",
                    cancellationToken);

            return reservas ?? [];
        }

        public async Task<IReadOnlyCollection<ReservaDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            var reservas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<ReservaDto>>(
                    "api/reservas",
                    cancellationToken);

            return reservas ?? [];
        }

        public async Task<(PresupuestoDto? Presupuesto, string? Error)> CalcularPresupuestoAsync(
            int funcionId,int cantidadButacas,CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            var response = await _httpClient.PostAsJsonAsync(
                "api/reservas/presupuesto",
                new { funcionId, cantidadButacas },
                cancellationToken);

            if (!response.IsSuccessStatusCode) {
                return (null, await response.Content.ReadAsStringAsync(cancellationToken));
            }

            var presupuesto = await response.Content.ReadFromJsonAsync<PresupuestoDto>(
                cancellationToken);

            return (presupuesto, null);
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
    }
}