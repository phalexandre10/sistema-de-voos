using ReservaPassagens.Controladores;
using ReservaPassagens.Data;
using ReservaPassagens.Modelos;

namespace ReservaPassagens.Serviço
{
    public interface IServicoVoo
    {
        Task<ResultadoConsultaVoo> ConsultarVoosAsync(RequisicaoConsultaVoo requisicao);
    }

    //public class ServicoVoo : IServicoVoo
    //{
    //    private readonly ContextoAereo _contexto;
    //    private readonly ILogger<ServicoVoo> _logger;

    //    public ServicoVoo(ContextoAereo contexto, ILogger<ServicoVoo> logger)
    //    {
    //        _contexto = contexto;
    //        _logger = logger;
    //    }

    //    public async Task<ResultadoConsultaVoo> ConsultarVoosAsync(RequisicaoConsultaVoo requisicao)
    //    {
    //        var voosIda = await BuscarVoos(requisicao.Origem, requisicao.Destino, requisicao.DataPartida);

    //        var resultado = new ResultadoConsultaVoo
    //        {
    //            VoosIda = voosIda
    //        };

    //        if (requisicao.DataRetorno.HasValue)
    //        {
    //            var voosVolta = await BuscarVoos(requisicao.Destino, requisicao.Origem, requisicao.DataRetorno.Value);
    //            resultado.VoosVolta = voosVolta;
    //        }

    //        return resultado;
    //    }

    //    private async Task<List<DtoVoo>> BuscarVoos(string origem, string destino, DateTime data)
    //    {
    //        return await _contexto.Voos
    //            .Where(v => v.Origem == origem &&
    //                        v.Destino == destino &&
    //                        v.HoraPartida.Date == data.Date)
    //            .Select(v => new DtoVoo
    //            {
    //                Id = v.Id,
    //                NumeroVoo = v.NumeroVoo,
    //                Origem = v.Origem,
    //                Destino = v.Destino,
    //                HoraPartida = v.HoraPartida,
    //                HoraChegada = v.HoraChegada,
    //                CompanhiaAerea = v.CompanhiaAerea,
    //                Preco = v.Preco
    //            })
    //            .ToListAsync();
    //    }
    }

}