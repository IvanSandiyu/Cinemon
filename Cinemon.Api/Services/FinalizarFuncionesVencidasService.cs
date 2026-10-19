using Cinemon.Application.Funciones.FinalizarFuncionVencida;
using MediatR;

namespace Cinemon.Api.Services
{
    /// <summary>
    /// Marca periódicamente como Finalizadas las funciones que ya terminaron
    /// (FechaHoraInicio + duración de la película &lt;= ahora).
    /// </summary>
    public sealed class FinalizarFuncionesVencidasService : BackgroundService
    {
        private static readonly TimeSpan Intervalo = TimeSpan.FromHours(3);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FinalizarFuncionesVencidasService> _logger;

        public FinalizarFuncionesVencidasService(IServiceScopeFactory scopeFactory,
            ILogger<FinalizarFuncionesVencidasService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try {
                    await FinalizarVencidasAsync(stoppingToken);
                } catch (OperationCanceledException) {
                    break;
                } catch (Exception ex) {
                    _logger.LogError(ex, "Error al finalizar funciones vencidas.");
                }

                try {
                    await Task.Delay(Intervalo, stoppingToken);
                } catch (TaskCanceledException) {
                    break;
                }
            }
        }

        private async Task FinalizarVencidasAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var cantidad = await sender.Send(
                new FinalizarFuncionesVencidasCommand(),
                cancellationToken);

            if (cantidad > 0) {
                _logger.LogInformation(
                    "Se marcaron {Cantidad} función(es) vencida(s) como Finalizadas.",
                    cantidad);
            }
        }
    }
}
