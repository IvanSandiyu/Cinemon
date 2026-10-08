using Cinemon.Application.Candy.Pedidos.Commands.CrearPedidoCandy;
using Cinemon.Application.Candy.Pedidos.Queries.ObtenerMisPedidosCandy;
using Cinemon.Application.DTOs.Candy;
using MediatR;
using System.Threading.Tasks;

namespace Cinemon.Api.Endpoints.Candy
{
    public static class PedidoCandyEndpoints
    {
        public static IEndpointRouteBuilder MapPedidoCandyEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/pedidos-candy")
                .WithTags("Candy");

            group.MapPost("/", CrearPedido)
                .RequireAuthorization(policy => policy.RequireRole("Cliente", "Admin"));

            group.MapGet("/mis-pedidos", MisPedidos)
                .RequireAuthorization(policy => policy.RequireRole("Cliente", "Admin"));

            return app;
        }

        private static async Task<IResult> CrearPedido(
            CrearPedidoCandyRequest request,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var id = await sender.Send(
                new CrearPedidoCandyCommand(request.Items),
                cancellationToken);

            return Results.Created($"/api/pedidos-candy/{id}", new { id });
        }

        private static async Task<IResult> MisPedidos(
            ISender sender,
            CancellationToken cancellationToken)
        {
            var pedidos = await sender.Send(new ObtenerMisPedidosCandyQuery(), cancellationToken);

            return Results.Ok(pedidos);
        }
    }
}
