using Cinemon.Application.Peliculas.Commands.CambiarEstadoPelicula;
using Cinemon.Application.Peliculas.Commands.CrearPelicula;
using Cinemon.Application.Peliculas.Commands.EditarPelicula;
using Cinemon.Application.Peliculas.Commands.VincularTmdb;
using Cinemon.Application.Peliculas.Queries.ObtenerPeliculas;

using MediatR;
using static Cinemon.Api.Endpoints.TmdbEndpoints;

namespace Cinemon.Api.Endpoints.Peliculas
{
    public static class PeliculaEndpoint
    {
        public static IEndpointRouteBuilder MapPeliculaEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/peliculas")
                .WithTags("Peliculas");

            group.MapPost("/", CrearPelicula).RequireAuthorization(policy =>policy.RequireRole("Admin"));

            group.MapGet("/", ObtenerPeliculas);

            group.MapGet("/{id}", ObtenerPeliculasId);

            group.MapPut("/{id}", EditarPeliculas).RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapPatch("/{id}/activar", ActivarPeliculas).RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapPatch("/{id}/desactivar", DesactivarPeliculas).RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapPost("/{id:int}/tmdb", VincularTmdb).RequireAuthorization(policy => policy.RequireRole("Admin"));

            return app;
        }

        private static async Task<IResult> ObtenerPeliculasId(int id,ISender sender, CancellationToken cancellationToken)
        {
            var query = new ObtenerPeliculasPorIdQuery(id);

            var pelicula = await sender.Send(query,cancellationToken);
            return pelicula is not null ? Results.Ok(pelicula) : Results.NotFound();

        }

        private static async Task<IResult> CrearPelicula(CrearPeliculaRequest request, ISender sender,CancellationToken cancellationToken)
        {
            var command = new CrearPeliculaCommand(
               request.Titulo,
               request.Sinopsis,
               request.Duracion,
               request.FechaEstreno,
               request.ClasificacionEdad,
               request.PosterUrl,
               request.TrailerUrl,
               request.GeneroIds);

            var peliculaId = await sender.Send(
              command,
              cancellationToken);

            return Results.Created($"/api/peliculas/{peliculaId}", peliculaId);

        }

        private static async Task<IResult> ObtenerPeliculas(ISender sender,CancellationToken cancellationToken)
        {
            var query = new ObtenerPeliculasQuery();

            var peliculas = await sender.Send(
                query,
                cancellationToken);

            return Results.Ok(peliculas);
        }

        private static async Task<IResult> EditarPeliculas(int id, EditarPeliculaRequest request,ISender sender, CancellationToken cancellationToken)
        {
            var command = new EditarPeliculaCommand(
                id,
                request.Titulo,
                request.Sinopsis,
                request.Duracion,
                request.FechaEstreno,
                request.ClasificacionEdad,
                request.PosterUrl,
                request.TrailerUrl,
                request.GeneroIds);

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> ActivarPeliculas(int id, ISender sender, CancellationToken cancellationToken)
        {
            var cambiada = await sender.Send(
                new CambiarEstadoPeliculaCommand(id, true),
                cancellationToken);

            return cambiada ? Results.NoContent() : Results.NotFound();
        }

        private static async Task<IResult> DesactivarPeliculas(int id, ISender sender, CancellationToken cancellationToken)
        {
            var cambiada = await sender.Send(
                new CambiarEstadoPeliculaCommand(id, false),
                cancellationToken);

            return cambiada ? Results.NoContent() : Results.NotFound();
        }

        private static async Task<IResult> VincularTmdb(int id, VincularTmdbRequest request, ISender sender, CancellationToken cancellationToken)
        {
            await sender.Send(
                new VincularTmdbCommand(id,request.TmdbId),cancellationToken);

            return Results.NoContent();
        }

        public sealed record VincularTmdbRequest(int TmdbId);
    }
}
