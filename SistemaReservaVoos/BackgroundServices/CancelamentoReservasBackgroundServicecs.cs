using SistemaReservasVoos.Servico;

namespace SistemaReservasVoos.BackgroundServices
{
    public class CancelamentoReservasBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public CancelamentoReservasBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var servicoVoo = scope.ServiceProvider.GetRequiredService<ServicoVoo>();
                    await servicoVoo.CancelarReservasNoShow();
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
