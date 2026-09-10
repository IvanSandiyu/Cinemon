using Cinemon.Web.Models.DTOs;
using System.Net.Http.Headers;

namespace Cinemon.Web.Services
{
    public sealed class UsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public UsuarioApiService(
            IHttpClientFactory httpClientFactory,
            AuthService authService)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
            _authService = authService;
        }

        public async Task<UsuarioDto?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();
            return await _httpClient.GetFromJsonAsync<UsuarioDto>(
                $"api/usuarios/{id}",
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<UsuarioAdminDto>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            AdjuntarToken();
            var usuarios = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<UsuarioAdminDto>>(
                    "api/usuarios",
                    cancellationToken);
            return usuarios ?? [];
        }

        private void AdjuntarToken()
        {
            if (!_authService.EstaAutenticado) return;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authService.Token);
        }
    }
}