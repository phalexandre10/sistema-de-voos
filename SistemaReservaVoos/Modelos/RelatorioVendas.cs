namespace SistemaReservasVoos.Modelos
{
    public class RelatorioVendas
    {
        public int VooId { get; set; }
        public decimal TotalVendas { get; set; }
        public decimal VendasCartao { get; set; }
        public decimal VendasTransferencia { get; set; }
        public decimal VendasDinheiro { get; set; }
        public decimal ComparacaoMesAnterior { get; set; }
    }
}

