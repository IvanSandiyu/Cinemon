using Cinemon.Application.Butacas.Queries.ObtenerButacasPorFuncion;
using Cinemon.Application.Butacas.Queries.ObtenerButacasPorSala;
using MediatR;

namespace Cinemon.Api.Endpoints.Butacas
{
    public static class ButacaEndpoints
    {
        public static IEndpointRouteBuilder MapButacaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/butacas")
                .WithTags("Butacas");

            group.MapGet("/sala/{salaId:int}", ObtenerButacasPorSala);
            group.MapGet("/funcion/{funcionId:int}", ObtenerButacasPorFuncion);

            return app;
        }

        private static async Task<IResult> ObtenerButacasPorFuncion(int funcionId,ISender sender,CancellationToken cancellationToken)
        {
            var result = await sender.Send(
            new ObtenerButacasPorFuncionQuery(funcionId),
            cancellationToken);

            return Results.Ok(result);
        }

        private static async Task<IResult> ObtenerButacasPorSala(
            int salaId,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var query = new ObtenerButacasPorSalaQuery(salaId);

            var butacas = await sender.Send(
                query,
                cancellationToken);

            return Results.Ok(butacas);
        }
    }
}
