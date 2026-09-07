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