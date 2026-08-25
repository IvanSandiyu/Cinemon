using Cinemon.Application.Peliculas.Commands.CrearPelicula;
using MediatR;

namespace Cinemon.Api.Endpoints.Peliculas
{
    public static class PeliculaEndpoint
    {
        public static void MapPeliculaEndpoint(
        this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/peliculas", async (
                CrearPeliculaRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
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

                return Results.Created(
                    $"/api/peliculas/{peliculaId}",
                    new
                    {
                        Id = peliculaId
                    });
            })
            .WithName("CrearPelicula")
            .WithTags("Peliculas")
            .WithOpenApi();
        }
    }
}
