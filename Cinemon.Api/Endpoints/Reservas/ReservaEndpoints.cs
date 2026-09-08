using Cinemon.Application.Interfaces;
using Cinemon.Application.Reservas.Commands.CancelarReserva;
using Cinemon.Application.Reservas.Commands.CrearReserva;
using Cinemon.Application.Reservas.Queries.ObtenerReservas;
using MediatR;

namespace Cinemon.Api.Endpoints.Reservas
{
    public static class ReservaEndpoints
    {
        public static IEndpointRouteBuilder MapReservaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/reservas")
                .WithTags("Reservas");
            
            group.MapPost("/", CrearReserva)
                .RequireAuthorization(policy => policy.RequireRole("Cliente", "Admin"));

            group.MapGet("/", ObtenerReservas)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapGet("/{id}", ObtenerReservaPorId)
                .RequireAuthorization(policy => policy.RequireRole("Cliente", "Admin"));

            group.MapPost("/{id}/cancelar", CancelarReserva)
                .RequireAuthorization(policy => policy.RequireRole("Cliente", "Admin"));

            group.MapGet("/mis-reservas", HistorialReservas).RequireAuthorization(policy => policy.RequireRole("Cliente", "Admin"));

            return app;
        }

        private static async Task<IResult> HistorialReservas(ICurrentUserService currentUserService,ISender sender,CancellationToken cancellationToken)
        {
            var reservas = await sender.Send(new ObtenerReservasPorUsuarioQuery(currentUserService.UserId),cancellationToken);

            return Results.Ok(reservas);
        }

        private static async Task<IResult> CrearReserva(CrearReservaCommand command,ISender sender,CancellationToken cancellationToken)
        {
            var reservaId = await sender.Send(command,cancellationToken);

            return Results.Created($"/api/reservas/{reservaId}",new {
                    Id = reservaId
                });
        }

        private static async Task<IResult> ObtenerReservas(ISender sender,CancellationToken cancellationToken)
        {
            var reservas = await sender.Send(new ObtenerReservasQuery(),cancellationToken);

            return Results.Ok(reservas);
        }

        private static async Task<IResult> ObtenerReservaPorId(int id,ISender sender,CancellationToken cancellationToken)
        {
            var reserva = await sender.Send(new ObtenerReservaPorIdQuery(id),cancellationToken);

            return reserva is not null? Results.Ok(reserva): Results.NotFound();
        }

        private static async Task<IResult> CancelarReserva(int id,ISender sender,CancellationToken cancellationToken)
        {
            var cancelada = await sender.Send(new CancelarReservaCommand(id),cancellationToken);

            return cancelada? Results.NoContent(): Results.NotFound();
        }
    }
}
