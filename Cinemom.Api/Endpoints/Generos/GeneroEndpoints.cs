using Cinemon.Application.Generos.Queries;
using MediatR;

namespace Cinemon.Api.Endpoints.Generos
{
    public static class GeneroEndpoints 
    {
        public static IEndpointRouteBuilder MapGeneroEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/generos")
                .WithTags("Generos");

            group.MapGet("/", ObtenerGeneros);

            return app;
        }

        private static async Task<IResult> ObtenerGeneros(ISender sender,CancellationToken cancellationToken)
        {
            var generos = await sender.Send(new ObtenerGenerosQuery(),cancellationToken);

            return Results.Ok(generos);
        }
    }
}
