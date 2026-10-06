using Cinemon.Application.DTOs.Promocion;
using Cinemon.Application.Promociones.Commands.ActualizarPromocion;
using Cinemon.Application.Promociones.Commands.CambiarEstadoPromocion;
using Cinemon.Application.Promociones.Commands.CrearPromocion;
using Cinemon.Application.Promociones.Commands.EliminarPromocion;
using Cinemon.Application.Promociones.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinemon.Api.Endpoints.Promociones
{
    public static class PromocionEndpoints
    {
        public static IEndpointRouteBuilder MapPromocionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/promociones")
                .WithTags("Promociones")
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapGet("/", ObtenerPromociones);
            group.MapPost("/", CrearPromocion);
            group.MapPut("/{id:int}", ActualizarPromocion);
            group.MapPatch("/{id:int}/estado", CambiarEstado);
            group.MapDelete("/{id:int}", EliminarPromocion);

            return app;
        }

        private static async Task<IResult> ObtenerPromociones(ISender sender,CancellationToken cancellationToken)
        {
            var promociones = await sender.Send(new ObtenerPromocionesQuery(),cancellationToken);

            return Results.Ok(promociones);
        }

        private static async Task<IResult> CrearPromocion(CrearPromocionRequest request,ISender sender,
            CancellationToken cancellationToken)
        {
            var id = await sender.Send(
                new CrearPromocionCommand(
                    request.Nombre,
                    request.Descripcion,
                    request.Tipo,
                    request.CantidadPagadas,
                    request.CantidadGratis,
                    request.PorcentajeDescuento,
                    request.FechaDesde,
                    request.FechaHasta,
                    request.DiasSemana),
                cancellationToken);

            return Results.Created($"/api/promociones/{id}",new { id });
        }

        private static async Task<IResult> ActualizarPromocion(int id,ActualizarPromocionRequest request,ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new ActualizarPromocionCommand(
                    id,
                    request.Nombre,
                    request.Descripcion,
                    request.Tipo,
                    request.CantidadPagadas,
                    request.CantidadGratis,
                    request.PorcentajeDescuento,
                    request.FechaDesde,
                    request.FechaHasta,
                    request.DiasSemana),
                cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> CambiarEstado(int id,CambiarEstadoPromocionRequest request,ISender sender,
            CancellationToken cancellationToken)
        {
            await sender.Send(
                new CambiarEstadoPromocionCommand(id,request.Activa),
                cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> EliminarPromocion(int id,ISender sender,CancellationToken cancellationToken)
        {
            await sender.Send(new EliminarPromocionCommand(id),cancellationToken);

            return Results.NoContent();
        }
    }

    public sealed record CambiarEstadoPromocionRequest(bool Activa);
}
