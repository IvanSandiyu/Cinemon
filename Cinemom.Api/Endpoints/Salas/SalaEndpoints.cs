using Cinemon.Application.Salas.Queries.ObtenerSalas;
using MediatR;

namespace Cinemon.Api.Endpoints.Salas
{
    public static class SalaEndpoints
    {
        public static IEndpointRouteBuilder MapSalaEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/salas")
                .WithTags("Salas");

            group.MapGet("/", ObtenerSalas);

            return app;
        }

        private static async Task<IResult> ObtenerSalas(
            ISender sender,
            CancellationToken cancellationToken)
        {
            var query = new ObtenerSalasQuery();

            var salas = await sender.Send(
                query,
                cancellationToken);

            return Results.Ok(salas);
        }
    }
}
