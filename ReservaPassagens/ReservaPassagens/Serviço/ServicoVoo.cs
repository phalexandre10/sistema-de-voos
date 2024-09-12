using Microsoft.EntityFrameworkCore;
using ReservaPassagens.Data;
namespace ReservaPassagens.Serviço
{


    public class ServicoVoo : IServicoVoo
    {
        private readonly ContextoAereo _contexto;
        private readonly ILogger<ServicoVoo> _logger;

        public ServicoVoo(ContextoAereo contexto, ILogger<ServicoVoo> logger)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<ResultadoConsultaVoo> ConsultarVoosAsync(RequisicaoConsultaVoo requisicao)
        {
            var voosIda = await _contexto.Voos
                .Where(f => f.Origem == requisicao.Origem &&
                            f.Destino == requisicao.Destino &&
                            f.HoraPartida.Date == requisicao.DataPartida.Date)
                .ToListAsync();

            var resultado = new ResultadoConsultaVoo
            {
                VoosIda = voosIda.Select(f => new DtoVoo
                {
                    Id = f.Id,
                    NumeroVoo = f.NumeroVoo,
                    Origem = f.Origem,
                    Destino = f.Destino,
                    HoraPartida = f.HoraPartida,
                    Preco = f.Preco
                }).ToList()
            };

            if (requisicao.DataRetorno.HasValue)
            {
                var voosVolta = await _contexto.Voos
                    .Where(f => f.Origem == requisicao.Destino &&
                                f.Destino == requisicao.Origem &&
                                f.HoraPartida.Date == requisicao.DataRetorno.Value.Date)
                    .ToListAsync();
                resultado.VoosVolta = voosVolta.Select(f => new DtoVoo
                {
                    Id = f.Id,
                    NumeroVoo = f.NumeroVoo,
                    Origem = f.Origem,
                    Destino = f.Destino,
                    HoraPartida = f.HoraPartida,
                    Preco = f.Preco
                }).ToList();
            }

            return resultado;
        }

    }
}
