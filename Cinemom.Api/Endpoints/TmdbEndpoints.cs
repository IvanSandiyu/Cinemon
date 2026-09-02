using Cinemon.Infrastructure.ExternalServices.Tmdb;
using Microsoft.AspNetCore.Mvc;

namespace Cinemon.Api.Endpoints
{
    public static class TmdbEndpoints
    {
        public static void MapTmdbEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/tmdb")
                .WithTags("TMDB");

            group.MapGet("/search", BuscarPeliculas);
        }
        
        private static async Task<IResult> BuscarPeliculas([FromQuery] string query,ITmdbService tmdbService,CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Results.BadRequest(
                    "Debe especificar una búsqueda.");

            var peliculas =
                await tmdbService.BuscarPeliculasAsync(
                    query,
                    cancellationToken);

            return Results.Ok(peliculas);
        }
    }
}
