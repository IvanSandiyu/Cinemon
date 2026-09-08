using Cinemon.Web.Models.DTOs;
using Cinemon.Web.Models.DTOs.Requests;
using System.Net.Http.Headers;

namespace Cinemon.Web.Services
{
    public sealed class FuncionApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public FuncionApiService(
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

        public async Task<IReadOnlyCollection<FuncionDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            var funciones = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<FuncionDto>>(
                    "api/funciones",
                    cancellationToken);

            return funciones ?? [];
        }

        public async Task<(int Id, string? Error)> CrearAsync(
            CrearFuncionRequest request)
        {
            AdjuntarToken();

            var response = await _httpClient.PostAsJsonAsync(
                "api/funciones",
                request);

            if (!response.IsSuccessStatusCode) {
                var contenido = await response.Content
                    .ReadAsStringAsync();

                return (0, contenido);
            }

            var creada = await response.Content
                .ReadFromJsonAsync<FuncionCreadaResponse>();

            return (creada?.Id ?? 0, null);
        }

        private sealed record FuncionCreadaResponse(int Id);
    }
}