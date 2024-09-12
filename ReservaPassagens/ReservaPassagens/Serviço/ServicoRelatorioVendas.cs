using ReservaPassagens.Data;

namespace ReservaPassagens.Serviço
{

    public class ServicoRelatorioVendas : IServicoRelatorioVendas
    {
        private readonly ContextoAereo _contexto;
        private readonly ILogger<ServicoRelatorioVendas> _logger;

        public ServicoRelatorioVendas(ContextoAereo contexto, ILogger<ServicoRelatorioVendas> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        public async Task<DtoRelatorioVendas> GerarRelatorioVendasAsync(int mes, int ano)
        {
            var dataInicio = new DateTime(ano, mes, 1);
            var dataFim = dataInicio.AddMonths(1).AddDays(-1);
            var dataInicioMesAnterior = dataInicio.AddMonths(-1);

            var vendasMesAtual = await ObterVendasPorPeriodo(dataInicio, dataFim);
            var vendasMesAnterior = await ObterVendasPorPeriodo(dataInicioMesAnterior, dataInicio.AddDays(-1));

            var relatorio = new DtoRelatorioVendas
            {
                Mes = mes,
                Ano = ano,
                VendasPorVoo = vendasMesAtual,
                TotalVendas = vendasMesAtual.Sum(v => v.TotalVendas),
                VendasPorFormaPagamento = await ObterVendasPorFormaPagamento(dataInicio, dataFim),
                ComparacaoMesAnterior = CalcularComparacaoMesAnterior(vendasMesAtual, vendasMesAnterior)
            };

            return relatorio;
        }

        private async Task<List<VendasPorVoo>> ObterVendasPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return await _contexto.Bilhetes
                .Where(b => b.Reserva.DataReserva >= dataInicio && b.Reserva.DataReserva <= dataFim)
                .GroupBy(b => b.Reserva.Voo.NumeroVoo)
                .Select(g => new VendasPorVoo
                {
                    NumeroVoo = g.Key,
                    TotalVendas = g.Sum(b => b.Preco)
                })
                .ToListAsync();
        }

        private async Task<List<VendasPorFormaPagamento>> ObterVendasPorFormaPagamento(DateTime dataInicio, DateTime dataFim)
        {
            return await _contexto.Bilhetes
                .Where(b => b.Reserva.DataReserva >= dataInicio && b.Reserva.DataReserva <= dataFim)
                .GroupBy(b => b.FormaPagamento)
                .Select(g => new VendasPorFormaPagamento
                {
                    FormaPagamento = g.Key,
                    TotalVendas = g.Sum(b => b.Preco)
                })
                .ToListAsync();
        }

        private double CalcularComparacaoMesAnterior(List<VendasPorVoo> vendasMesAtual, List<VendasPorVoo> vendasMesAnterior)
        {
            var totalMesAtual = vendasMesAtual.Sum(v => v.TotalVendas);
            var totalMesAnterior = vendasMesAnterior.Sum(v => v.TotalVendas);

            if (totalMesAnterior == 0)
                return 100; // Aumento de 100% se não houve vendas no mês anterior

            return (totalMesAtual - totalMesAnterior) / totalMesAnterior * 100;
        }
    }

}
