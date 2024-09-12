using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservaPassagens.Data;
using ReservaPassagens.Modelos;

namespace ReservaPassagens.Serviço
{


    public class ServicoPassageiro : IServicoPassageiro
    {
        private readonly ContextoAereo _contexto;
        private readonly IMapper _mapeador;
        private readonly ILogger<ServicoPassageiro> _logger;

        public ServicoPassageiro(ContextoAereo contexto, IMapper mapeador, ILogger<ServicoPassageiro> logger)
        {
            _contexto = contexto;
            _mapeador = mapeador;
            _logger = logger;
        }

        public async Task<bool> VerificarCPFExistenteAsync(string cpf)
        {
            return await _contexto.Passageiros.AnyAsync(p => p.CPF == cpf);
        }

        public async Task<DtoPassageiro> RegistrarPassageiroAsync(RequisicaoRegistroPassageiro requisicao)
        {
            var passageiroExistente = await _contexto.Passageiros.FirstOrDefaultAsync(p => p.CPF == requisicao.CPF);
            if (passageiroExistente != null)
            {
                throw new ExcecaoPassageiroJaExiste(requisicao.CPF);
            }

            var novoPassageiro = _mapeador.Map<Passageiro>(requisicao);
            _contexto.Passageiros.Add(novoPassageiro);
            await _contexto.SaveChangesAsync();

            _logger.LogInformation("Novo passageiro registrado com CPF: {CPF}", novoPassageiro.CPF);

            return _mapeador.Map<DtoPassageiro>(novoPassageiro);
        }

        public async Task<DtoPassageiro> ObterPassageiroAsync(int id)
        {
            var passageiro = await _contexto.Passageiros.FindAsync(id);
            return passageiro != null ? _mapeador.Map<DtoPassageiro>(passageiro) : null;
        }

        public async Task<ResultadoPaginado<DtoPassageiro>> ObterPassageirosAsync(ParametrosPaginacao parametrosPaginacao)
        {
            var consulta = _contexto.Passageiros.AsQueryable();
            var contagemTotal = await consulta.CountAsync();

            var passageiros = await consulta
                .Skip((parametrosPaginacao.NumeroPagina - 1) * parametrosPaginacao.TamanhoPagina)
                .Take(parametrosPaginacao.TamanhoPagina)
                .ToListAsync();

            var dtosPassageiros = _mapeador.Map<List<DtoPassageiro>>(passageiros);

            return new ResultadoPaginado<DtoPassageiro>
            {
                Itens = dtosPassageiros,
                ContagemTotal = contagemTotal,
                NumeroPagina = parametrosPaginacao.NumeroPagina,
                TamanhoPagina = parametrosPaginacao.TamanhoPagina
            };
        }
    }

  

}
