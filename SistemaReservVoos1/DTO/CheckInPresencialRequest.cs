using SistemaReservasVoos.Modelos;

namespace SistemaReservasVoos.DTO
{
    public class CheckInPresencialRequest
    {
        public int ReservaId { get; set; }
        public string NumeroAssento { get; set; }
        public Passageiro DadosAtualizados { get; set; }
    }
}
