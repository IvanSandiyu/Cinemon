using Cinemon.Web.Models.DTOs.Requests;

namespace Cinemon.Web.Services
{
    public sealed class ReservaApiService
    {
        private readonly HttpClient _httpClient;

        public ReservaApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }

        public async Task<HttpResponseMessage> CrearAsync(CrearReservaRequest request,CancellationToken cancellationToken = default)
        {
            return await _httpClient.PostAsJsonAsync(
                "api/reservas",
                request,
                cancellationToken);
        }
    }
}
