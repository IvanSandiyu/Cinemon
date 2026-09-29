using Cinemon.Application.Precios.Commands.ActualizarPrecio;
using Cinemon.Application.Precios.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Api.Endpoints.Precios
{
    public static class PrecioEndpoints
    {
        public static IEndpointRouteBuilder MapPrecioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/precios")
                .WithTags("Precios");

            group.MapGet("/", ObtenerPrecios);
            group.MapPut("/{id:int}", ActualizarPrecio)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            return app;
        }

        private static async Task<IResult> ObtenerPrecios(ISender sender,CancellationToken cancellationToken)
        {
            var precios = await sender.Send(new ObtenerPreciosQuery(),cancellationToken);

            return Results.Ok(precios);
        }

        private static async Task<IResult> ActualizarPrecio(int id,ActualizarPrecioRequest request,ISender sender,
            CancellationToken cancellationToken)
        {
            var command = new ActualizarPrecioCommand(
                id,
                request.Valor);

            await sender.Send(command,cancellationToken);

            return Results.NoContent();
        }
    }
}
