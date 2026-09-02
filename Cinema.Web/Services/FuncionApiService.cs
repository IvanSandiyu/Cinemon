using Cinemon.Web.Models.DTOs;

namespace Cinemon.Web.Services
{
    public sealed class FuncionApiService
    {
        private readonly HttpClient _httpClient;

        public FuncionApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");;
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
    }
}
