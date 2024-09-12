namespace ReservaPassagens.DTOs
{

    // Update DTOs
    public class DtoBilhete
    {
        public string NumeroBilhete { get; set; }
        public string NomePassageiro { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime DataHoraPartida { get; set; }
        public string NumeroVoo { get; set; }
        public string NumeroAssento { get; set; }
        public decimal Preco { get; set; }
    }
}

