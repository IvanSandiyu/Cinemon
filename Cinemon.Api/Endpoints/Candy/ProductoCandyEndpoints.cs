using Cinemon.Application.Candy.Productos.Commands.ActualizarProductoCandy;
using Cinemon.Application.Candy.Productos.Commands.CambiarEstadoProductoCandy;
using Cinemon.Application.Candy.Productos.Commands.CrearProductoCandy;
using Cinemon.Application.Candy.Productos.Commands.EliminarProductoCandy;
using Cinemon.Application.Candy.Productos.Queries.ObtenerProductosCandy;
using Cinemon.Application.DTOs.Candy;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinemon.Api.Endpoints.Candy
{
    public static class ProductoCandyEndpoints
    {
        public static IEndpointRouteBuilder MapProductoCandyEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/productos-candy")
                .WithTags("Candy");

            group.MapGet("/", ObtenerProductos);

            group.MapGet("/admin", ObtenerTodosLosProductos)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapPost("/", CrearProducto)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapPut("/{id:int}", ActualizarProducto)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapPatch("/{id:int}/estado", CambiarEstado)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapDelete("/{id:int}", EliminarProducto)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            return app;
        }

        private static async Task<IResult> ObtenerProductos(
            ISender sender,
            CancellationToken cancellationToken)
        {
            var productos = await sender.Send(
                new ObtenerProductosCandyQuery(IncluirInactivos: false),
                cancellationToken);

            return Results.Ok(productos);
        }

        private static async Task<IResult> ObtenerTodosLosProductos(
            ISender sender,
            CancellationToken cancellationToken)
        {
            var productos = await sender.Send(
                new ObtenerProductosCandyQuery(IncluirInactivos: true),
                cancellationToken);

            return Results.Ok(productos);
        }

        private static async Task<IResult> CrearProducto(
            CrearProductoCandyRequest request,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var id = await sender.Send(
                new CrearProductoCandyCommand(
                    request.Nombre,
                    request.Descripcion,
                    request.Precio,
                    request.Categoria,
                    request.Componentes),
                cancellationToken);

            return Results.Created($"/api/productos-candy/{id}", new { id });
        }

        private static async Task<IResult> ActualizarProducto(
            int id,
            ActualizarProductoCandyRequest request,
            ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new ActualizarProductoCandyCommand(
                    id,
                    request.Nombre,
                    request.Descripcion,
                    request.Precio,
                    request.Categoria,
                    request.Componentes),
                cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> CambiarEstado(
            int id,
            CambiarEstadoProductoCandyRequest request,
            ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new CambiarEstadoProductoCandyCommand(id, request.Activo),
                cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> EliminarProducto(
            int id,
            ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(new EliminarProductoCandyCommand(id), cancellationToken);

            return Results.NoContent();
        }
    }

    public sealed record CambiarEstadoProductoCandyRequest(bool Activo);
}
