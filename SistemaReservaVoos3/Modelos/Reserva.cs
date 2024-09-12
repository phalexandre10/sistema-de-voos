namespace SistemaReservasVoos.Modelos
{
    public class Reserva
    {
        public int Id { get; set; }
        public string CPFPassageiro { get; set; }
        public Passageiro Passageiro { get; set; }
        public int VooId { get; set; }
        public Voo Voo { get; set; }
        public DateTime DataReserva { get; set; }
        public int QuantidadePassageiros { get; set; }
        public string NumeroAssento { get; set; }
        public bool CheckInRealizado { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
    }
}
