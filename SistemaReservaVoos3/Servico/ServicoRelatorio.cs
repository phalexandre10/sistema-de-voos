using SistemaReservasVoos.Data;
using SistemaReservasVoos.Modelos;
using SistemaReservasVoos.Extensions;
using SistemaReservasVoos.Extensions;
using Microsoft.EntityFrameworkCore;
namespace SistemaReservasVoos.Servico
{
    public class ServicoRelatorio
    {
        private readonly SistemaReservasContext _db;

        public ServicoRelatorio(SistemaReservasContext db)
        {
            _db = db;
        }

        public async Task<List<RelatorioOcupacao>> GerarRelatorioOcupacaoSemanal()
        {
            var dataInicio = DateTime.Now.AddDays(-7);
            var relatorio = await _db.Voos
                .Where(v => v.DataPartida >= dataInicio)
                .Select(v => new RelatorioOcupacao
                {
                    VooId = v.Id,
                    Origem = v.Origem,
                    Destino = v.Destino,
                    DataPartida = v.DataPartida,
                    Companhia = v.Companhia,
                    PercentualOcupacao = 100 - ((double)v.AssentosDisponiveis / v.CapacidadeTotal * 100)
                })
                .ToListAsync();

            return relatorio;
        }

        public async Task<List<RelatorioVendas>> GerarRelatorioVendasMensal()
        {
            var mesAtual = DateTime.Now.Month;
            var anoAtual = DateTime.Now.Year;
            var mesAnterior = mesAtual == 1 ? 12 : mesAtual - 1;
            var anoAnterior = mesAtual == 1 ? anoAtual - 1 : anoAtual;

            var vendasMesAtual = await _db.Reservas
                .Where(r => r.DataReserva.Month == mesAtual && r.DataReserva.Year == anoAtual)
                .GroupBy(r => r.VooId)
                .Select(g => new
                {
                    VooId = g.Key,
                    TotalVendas = g.Sum(r => r.Voo.Preco * r.QuantidadePassageiros),
                    VendasCartao = g.Sum(r => r.FormaPagamento == FormaPagamento.Cartao ? r.Voo.Preco * r.QuantidadePassageiros : 0),
                    VendasTransferencia = g.Sum(r => r.FormaPagamento == FormaPagamento.Transferencia ? r.Voo.Preco * r.QuantidadePassageiros : 0),
                    VendasDinheiro = g.Sum(r => r.FormaPagamento == FormaPagamento.Dinheiro ? r.Voo.Preco * r.QuantidadePassageiros : 0)
                })
                .ToListAsync();

            var vendasMesAnterior = await _db.Reservas
                .Where(r => r.DataReserva.Month == mesAnterior && r.DataReserva.Year == anoAnterior)
                .GroupBy(r => r.VooId)
                .Select(g => new
                {
                    VooId = g.Key,
                    TotalVendas = g.Sum(r => r.Voo.Preco * r.QuantidadePassageiros)
                })
                .ToListAsync();

            var relatorio = vendasMesAtual.Select(v => new RelatorioVendas
            {
                VooId = v.VooId,
                TotalVendas = v.TotalVendas,
                VendasCartao = v.VendasCartao,
                VendasTransferencia = v.VendasTransferencia,
                VendasDinheiro = v.VendasDinheiro,
                ComparacaoMesAnterior = v.TotalVendas - (vendasMesAnterior.FirstOrDefault(va => va.VooId == v.VooId)?.TotalVendas ?? 0)
            }).ToList();

            return relatorio;
        }
    }

    // Serviço em segundo plano para cancelar reservas não confirmadas
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
