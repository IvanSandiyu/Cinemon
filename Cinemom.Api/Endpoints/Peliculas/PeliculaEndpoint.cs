using Cinemon.Application.Peliculas.Commands.CrearPelicula;
using Cinemon.Application.Peliculas.Queries.ObtenerPeliculas;

using MediatR;

namespace Cinemon.Api.Endpoints.Peliculas
{
    public static class PeliculaEndpoint
    {
        public static IEndpointRouteBuilder MapPeliculaEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/peliculas")
                .WithTags("Peliculas");

            group.MapPost("/", CrearPelicula);

            group.MapGet("/", ObtenerPeliculas);

            group.MapGet("/{id}", ObtenerPeliculasId);

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

            return Results.Created();

        }

        private static async Task<IResult> ObtenerPeliculas(ISender sender,CancellationToken cancellationToken)
        {
            var query = new ObtenerPeliculasQuery();

            var peliculas = await sender.Send(
                query,
                cancellationToken);

            return Results.Ok(peliculas);
        }

       
    }
}
