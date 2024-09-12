namespace SistemaReservasVoos.DTO
{
    public class ReservaRequest
    {
        public int VooId { get; set; }
        public string CPFPassageiro { get; set; }
        public int QuantidadePassageiros { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
    }
}
