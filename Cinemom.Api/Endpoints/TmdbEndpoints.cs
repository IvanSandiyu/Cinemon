using Cinemon.Application.Peliculas.Commands.VincularTmdb;
using Cinemon.Application.Tmdb.Queries.BuscarPeliculasTmdb;
using Cinemon.Infrastructure.ExternalServices.Tmdb;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cinemon.Api.Endpoints
{
    public static class TmdbEndpoints
    {
        public static void MapTmdbEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/tmdb")
                .WithTags("TMDB")
                .RequireAuthorization(policy =>policy.RequireRole("Admin"));

            group.MapGet("/search", BuscarPeliculas);
        }

        private static async Task<IResult> BuscarPeliculas([FromQuery] string query,ISender sender,CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Results.BadRequest(
                    "Debe especificar una búsqueda.");

            var result = await sender.Send(
                new BuscarPeliculasTmdbQuery(query),cancellationToken);

            return Results.Ok(result);
        }
    }
}
