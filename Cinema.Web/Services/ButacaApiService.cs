using Cinemon.Web.Models.DTOs;
using System.Net.Http;

namespace Cinemon.Web.Services
{
    public sealed class ButacaApiService
    {
        private readonly HttpClient _httpClient;

        public ButacaApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }

        public async Task<IReadOnlyCollection<ButacaDto>> ObtenerPorSalaAsync(int salaId,CancellationToken cancellationToken = default)
        {
            var butacas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<ButacaDto>>(
                    $"api/butacas/sala/{salaId}",
                    cancellationToken);

            return butacas ?? [];
        }

        public async Task<IReadOnlyCollection<ButacaFuncionDto>> ObtenerPorFuncionAsync(int funcionId,CancellationToken cancellationToken = default)
        {
            var butacas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<ButacaFuncionDto>>(
                    $"api/butacas/funcion/{funcionId}",
                    cancellationToken);

            return butacas ?? [];
        }
    }
}
