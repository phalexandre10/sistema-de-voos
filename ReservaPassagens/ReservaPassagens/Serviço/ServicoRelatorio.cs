using ReservaPassagens.Data;

namespace ReservaPassagens.Serviço
{
    public class ServicoRelatorio : IServicoRelatorio
    {
        private readonly ContextoAereo _contexto;
        private readonly ILogger<ServicoRelatorio> _logger;

        public ServicoRelatorio(ContextoAereo contexto, ILogger<ServicoRelatorio> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        public async Task<DtoRelatorioOcupacao> GerarRelatorioOcupacaoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var voos = await _contexto.Voos
                .Where(v => v.HoraPartida >= dataInicio && v.HoraPartida <= dataFim)
                .Include(v => v.Assentos)
                .ToListAsync();

            var relatorio = new DtoRelatorioOcupacao
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                OcupacaoPorVoo = voos.Select(v => new OcupacaoVoo
                {
                    NumeroVoo = v.NumeroVoo,
                    DataPartida = v.HoraPartida,
                    PorcentagemOcupacao = (double)v.Assentos.Count(a => a.EstaOcupado) / v.Assentos.Count * 100
                }).ToList()
            };

            return relatorio;
        }
    }

}
