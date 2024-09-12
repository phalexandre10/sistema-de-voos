namespace ReservaPassagens.Serviço
{
    public interface IServicoRelatorio
    {
        Task<DtoRelatorioOcupacao> GerarRelatorioOcupacaoAsync(DateTime dataInicio, DateTime dataFim);
    }
}
