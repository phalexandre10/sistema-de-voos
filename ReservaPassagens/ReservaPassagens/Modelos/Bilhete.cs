namespace ReservaPassagens.Modelos
{

    public class Bilhete
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }
        public string NumeroBilhete { get; set; }
        public decimal Preco { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
    }
}
