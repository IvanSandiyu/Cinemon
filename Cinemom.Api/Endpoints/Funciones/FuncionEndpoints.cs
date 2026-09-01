using Cinemon.Application.Funciones.Commands.CrearFuncion;
using Cinemon.Application.Funciones.Queries.ObtenerFunciones;
using MediatR;

namespace Cinemon.Api.Endpoints.Funciones
{
    public static class FuncionEndpoints
    {
        public static IEndpointRouteBuilder MapFuncionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/funciones")
                .WithTags("Funciones");

            group.MapPost("/", CrearFuncion).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            group.MapGet("/", ObtenerFunciones);

            return app;
        }

        private static async Task<IResult> CrearFuncion(
            CrearFuncionCommand command,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var funcionId = await sender.Send(
                command,
                cancellationToken);

            return Results.Created(
                $"/api/funciones/{funcionId}",
                new
                {
                    id = funcionId
                });
        }

        private static async Task<IResult> ObtenerFunciones(ISender sender, CancellationToken cancellationToken)
        {
            var query = new ObtenerFuncionesQuery();
            var funciones = await sender.Send(query, cancellationToken);
            return Results.Ok(funciones);
        }
    }
}
