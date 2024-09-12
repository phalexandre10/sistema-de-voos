namespace SistemaReservasVoos.Modelos
{
    public class Voo
    {
        public int Id { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime DataPartida { get; set; }
        public TimeSpan Horario { get; set; }
        public string Companhia { get; set; }
        public decimal Preco { get; set; }
        public int AssentosDisponiveis { get; set; }
        public int CapacidadeTotal { get; set; } = 100;

    }

}
