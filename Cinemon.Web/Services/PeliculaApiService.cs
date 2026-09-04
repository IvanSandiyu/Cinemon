using Cinemon.Web.Models.DTOs;
using Cinemon.Web.Models.DTOs.Requests;
using System.Net.Http;

namespace Cinemon.Web.Services
{
    public sealed class PeliculaApiService
    {
        private readonly HttpClient _httpClient;
        public PeliculaApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CinemonApi");
        }

        public async Task<IReadOnlyCollection<PeliculaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
        {
            var peliculas = await _httpClient.GetFromJsonAsync<
                IReadOnlyCollection<PeliculaDto>>(
                    "api/peliculas",
                    cancellationToken);

            return peliculas ?? [];
        }

        public async Task<PeliculaDto?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<PeliculaDto>(
                $"api/peliculas/{id}",
                cancellationToken);
        }

        public async Task ActivarAsync(int id)
        {
            var response = await _httpClient.PatchAsync(
                $"api/peliculas/{id}/activar",
                null);

            response.EnsureSuccessStatusCode();
        }

        public async Task DesactivarAsync(int id)
        {
            var response = await _httpClient.PatchAsync(
                $"api/peliculas/{id}/desactivar",
                null);

            response.EnsureSuccessStatusCode();
        }

        public async Task<int> CrearAsync(CrearPeliculaRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/peliculas",
                request);

            response.EnsureSuccessStatusCode();

            var peliculaId = await response.Content.ReadFromJsonAsync<int>();

            return peliculaId;
        }


    }
}
