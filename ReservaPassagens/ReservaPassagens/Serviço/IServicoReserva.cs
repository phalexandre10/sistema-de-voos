namespace ReservaPassagens.Serviço
{
    public interface IServicoReserva
    {
        Task<DtoReserva> RealizarReservaAsync(RequisicaoReserva requisicao);
        Task<DtoReserva> ObterReservaAsync(int id);
        Task<DtoReserva> CriarReservaAsync(RequisicaoReserva requisicao);
        Task<DtoReserva> CancelarReservaAsync(int id);
        Task CancelarReservasExpiradas();
    }
}
