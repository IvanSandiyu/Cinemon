using Cinemon.Application.Funciones.Commands.CancelarFuncion;
using Cinemon.Application.Funciones.Commands.CrearFuncion;
using Cinemon.Application.Funciones.Commands.EditarFuncion;
using Cinemon.Application.Funciones.Commands.FinalizarFuncion;
using Cinemon.Application.Funciones.Commands.CrearProgramacion;
using Cinemon.Application.Funciones.Queries.ObtenerFunciones;
using MediatR;
using System.Threading.Tasks.Dataflow;

namespace Cinemon.Api.Endpoints.Funciones
{
    public static class FuncionEndpoints
    {
        public static IEndpointRouteBuilder MapFuncionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/funciones")
                .WithTags("Funciones");

            group.MapPost("/", CrearFuncion).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            group.MapPost("/programacion", CrearProgramacion).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            group.MapGet("/", ObtenerFunciones);
            group.MapPut("/{id:int}", EditarFuncion).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            group.MapPatch("/{id:int}/cancelar", CancelarFuncion).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            group.MapPatch("/{id:int}/finalizar", FinalizarFuncion).RequireAuthorization(policy =>policy.RequireRole("Admin"));
            return app;
        }

        private static async Task<IResult> EditarFuncion(int id,FuncionDto request,ISender sender,CancellationToken cancellationToken)
        {
            var command = new EditarFuncionCommand(
                id,
                request.PeliculaId,
                request.SalaId,
                request.FechaHoraInicio,
                request.Idioma,
                request.Formato,
                request.Precio);

            await sender.Send(command, cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> FinalizarFuncion(int id,ISender sender,CancellationToken cancellationToken)
        {
            await sender.Send(new FinalizarFuncionCommand(id),cancellationToken);
            return Results.NoContent();
        }

        private static async Task<IResult> CancelarFuncion(int id,ISender sender,CancellationToken cancellationToken)
        {
            await sender.Send( new CancelarFuncionCommand(id), cancellationToken);
            return Results.NoContent();
        }

        private static async Task<IResult> CrearFuncion(CrearFuncionCommand command,ISender sender,CancellationToken cancellationToken)
        {
            var funcionId = await sender.Send(command,cancellationToken);

            return Results.Created($"/api/funciones/{funcionId}",new{
                    id = funcionId
                });
        }

        private static async Task<IResult> CrearProgramacion(CrearProgramacionRequest request,ISender sender,
            CancellationToken cancellationToken)
        {
            var diasSemana = request.DiasSemana
                .Where(d => Enum.IsDefined(typeof(DayOfWeek),d))
                .Select(d => (DayOfWeek)d)
                .Distinct()
                .ToList();

            if (diasSemana.Count == 0) {
                return Results.BadRequest(new {
                        title = "Datos inválidos",
                        detail = "Seleccioná al menos un día de la semana válido."
                    });
            }

            var command = new CrearProgramacionCommand(
                request.PeliculaId,
                request.SalaId,
                request.FechaInicio,
                diasSemana,
                request.CantidadSemanas,
                request.Idioma,
                request.Formato,
                request.Precio);

            var resultado = await sender.Send(command,cancellationToken);

            return Results.Ok(new ProgramacionResponse(
                resultado.Creadas
                    .Select(x => new ProgramacionItemResponse(x.Id,x.FechaHoraInicio))
                    .ToList(),
                resultado.Omitidas.ToList()));
        }

        private static async Task<IResult> ObtenerFunciones(ISender sender, CancellationToken cancellationToken)
        {
            var query = new ObtenerFuncionesQuery();
            var funciones = await sender.Send(query, cancellationToken);
            return Results.Ok(funciones);
        }
    }
}
