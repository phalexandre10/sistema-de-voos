namespace SistemaReservasVoos.Modelos
{
    public class RelatorioOcupacao
    {
        public int VooId { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime DataPartida { get; set; }
        public string Companhia { get; set; }
        public double PercentualOcupacao { get; set; }
    }

}
