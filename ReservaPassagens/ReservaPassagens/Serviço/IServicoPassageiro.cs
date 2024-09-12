namespace ReservaPassagens.Serviço;


// Services
public interface IServicoPassageiro
{
    Task<DtoPassageiro> RegistrarPassageiroAsync(RequisicaoRegistroPassageiro requisicao);
    Task<DtoPassageiro> ObterPassageiroAsync(int id);
    Task<ResultadoPaginado<DtoPassageiro>> ObterPassageirosAsync(ParametrosPaginacao parametrosPaginacao);
}
