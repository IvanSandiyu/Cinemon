using Cinemon.Web.Models.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Cinemon.Web.Services
{
    public sealed class CandyApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public CandyApiService(
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

        public async Task<IReadOnlyCollection<ProductoCandyDto>> ObtenerDisponiblesAsync(
            CancellationToken cancellationToken = default)
        {
            var productos = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<ProductoCandyDto>>(
                    "api/productos-candy",
                    cancellationToken);

            return productos ?? [];
        }

        public async Task<IReadOnlyCollection<ProductoCandyDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            var productos = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<ProductoCandyDto>>(
                    "api/productos-candy/admin",
                    cancellationToken);

            return productos ?? [];
        }

        public async Task<(bool Ok, string? Error)> CrearAsync(
            CrearProductoCandyRequest request)
        {
            AdjuntarToken();

            var response = await _httpClient.PostAsJsonAsync(
                "api/productos-candy",
                request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Ok, string? Error)> ActualizarAsync(
            ActualizarProductoCandyRequest request)
        {
            AdjuntarToken();

            var response = await _httpClient.PutAsJsonAsync(
                $"api/productos-candy/{request.Id}",
                request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Ok, string? Error)> CambiarEstadoAsync(
            int id,
            bool activo)
        {
            AdjuntarToken();

            var response = await _httpClient.PatchAsJsonAsync(
                $"api/productos-candy/{id}/estado",
                new { activo });

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Ok, string? Error)> EliminarAsync(int id)
        {
            AdjuntarToken();

            var response = await _httpClient.DeleteAsync($"api/productos-candy/{id}");

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(int? Id, string? Error)> CrearPedidoAsync(
            IReadOnlyCollection<CrearPedidoCandyItemRequest> items,
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            var response = await _httpClient.PostAsJsonAsync(
                "api/pedidos-candy",
                new CrearPedidoCandyRequest(items),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (null, await response.Content.ReadAsStringAsync(cancellationToken));

            var creado = await response.Content.ReadFromJsonAsync<CreadoDto>(cancellationToken);

            return (creado?.Id, null);
        }

        public async Task<IReadOnlyCollection<PedidoCandyDto>> ObtenerMisPedidosAsync(
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();

            var pedidos = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<PedidoCandyDto>>(
                    "api/pedidos-candy/mis-pedidos",
                    cancellationToken);

            return pedidos ?? [];
        }

        private sealed record CreadoDto(int Id);
    }
}
