using BiciRodriguez.Api.Services;
using BiciRodriguez.Api.Workers;
using BiciRodriguez.Api.Interfaces;

namespace BiciRodriguez.Api.Workers
{
    public class CierreCajaWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public CierreCajaWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Calcula cuánto falta para la medianoche
                var ahora = DateTime.Now;
                var proximaMedianoche = DateTime.Today.AddDays(1);
                var tiempoEspera = proximaMedianoche - ahora;

                // Espera hasta la medianoche
                await Task.Delay(tiempoEspera, stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var balanceService = scope.ServiceProvider.GetRequiredService<IBalancesService>();
                    // Ejecuta el cierre (usuarioId null porque es automático del sistema)
                    await balanceService.GenerarCierreDiarioAsync(null);
                }
            }
        }
    }
}