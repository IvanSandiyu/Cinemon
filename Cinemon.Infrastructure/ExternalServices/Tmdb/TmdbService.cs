using Cinemon.Application.Interfaces;
using Cinemon.Application.DTOs.Tmdb;
using Cinemon.Infrastructure.ExternalServices.Tmdb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Cinemon.Application.Common;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb
{
    public sealed class TmdbService : ITmdbService, ITmdbImageUrlBuilder
    {
        private readonly HttpClient _httpClient;

        public TmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<TmdbMovieDto>> BuscarPeliculasAsync(string query, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbMovieSearchResponse>(
                $"search/movie?query={Uri.EscapeDataString(query)}&language=es-AR",
                cancellationToken);

            if (response is null)
                return [];

            return response.Results.Select(movie => new TmdbMovieDto(
                    movie.Id,
                    movie.Title,
                    movie.Overview,
                    movie.PosterPath,
                    movie.BackdropPath,
                    ParseReleaseDate(movie.ReleaseDate),
                    null,
                    null,
                    []))
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
        public async Task<TmdbMovieDto?> ObtenerPeliculaAsync(int tmdbId, CancellationToken cancellationToken)
        {
            var movie = await _httpClient.GetFromJsonAsync<TmdbMovieResponse>(
                $"movie/{tmdbId}?language=es-AR&append_to_response=release_dates",cancellationToken);

            if (movie is null)
                return null;

            var generos = movie.Generos
                ?.Select(x => x.Nombre)
                .ToList() ?? [];

            return new TmdbMovieDto(
                movie.Id,
                movie.Title,
                movie.Overview,
                movie.PosterPath,
                movie.BackdropPath,
                ParseReleaseDate(movie.ReleaseDate),
                movie.Runtime,
                ExtractCertificacion(movie.ReleaseDates),
                generos);
        }

        private static string? ExtractCertificacion(TmdbReleaseDatesResponse? releaseDates)
        {
            var paises = releaseDates?.Results ?? [];

            var argentina = BuscarCertificacion(paises, "AR");
            if (argentina is not null)
                return argentina;

            var estadosUnidos = BuscarCertificacion(paises, "US");
            if (estadosUnidos is not null)
                return MapearClasificacionUs(estadosUnidos);

            return paises
                .SelectMany(x => x.ReleaseDates ?? [])
                .Select(x => x.Certification)
                .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c));
        }

        private static string? BuscarCertificacion(
            IReadOnlyCollection<TmdbCountryRelease> paises,
            string codigo)
        {
            return paises
                .Where(x => string.Equals(
                    x.Iso31661,
                    codigo,
                    StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.ReleaseDates ?? [])
                .Select(x => x.Certification)
                .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c));
        }

        private static string? MapearClasificacionUs(string certificacion)
        {
            return certificacion.Trim().ToUpperInvariant() switch
            {
                "G" => "ATP",
                "PG" => "+13",
                "PG-13" or "PG13" => "+13",
                "R" => "+18",
                "NC-17" or "NC17" => "+18",
                _ => certificacion
            };
        }

        public string? ObtenerPosterUrl(string? posterPath)
        {
            return TmdbImageUrlHelper.BuildPosterUrl(posterPath);
        }

        public string? ObtenerBackdropUrl(string? backdropPath)
        {
            return TmdbImageUrlHelper.BuildBackdropUrl(backdropPath);
        }


    }
}
