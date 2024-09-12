namespace ReservaPassagens.Serviço
{
    public interface IServicoRelatorioVendas
    {
        Task<DtoRelatorioVendas> GerarRelatorioVendasAsync(int mes, int ano);
    }

}
