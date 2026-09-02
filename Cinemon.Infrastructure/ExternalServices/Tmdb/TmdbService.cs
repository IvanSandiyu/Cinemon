using Cinemon.Application.DTOs.Tmdb;
using Cinemon.Infrastructure.ExternalServices.Tmdb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb
{
    public sealed class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;

        public TmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<TmdbMovieDto>> BuscarPeliculasAsync(string query,CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbMovieSearchResponse>(
                $"search/movie?query={Uri.EscapeDataString(query)}&language=es-AR",
                cancellationToken);

            if (response is null)
                return [];

            return response.Results
                .Select(movie => new TmdbMovieDto(
                    movie.Id,
                    movie.Title,
                    movie.Overview,
                    movie.PosterPath,
                    movie.BackdropPath,
                    ParseReleaseDate(movie.ReleaseDate)))
                .ToList();
        }

        private static DateTime? ParseReleaseDate(string? releaseDate)
        {
            if (DateTime.TryParse(
                releaseDate,
                out var date)) {
                return date;
            }

            return null;
        }

       
    }
}
