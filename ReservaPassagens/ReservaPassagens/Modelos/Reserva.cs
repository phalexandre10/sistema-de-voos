using System.ComponentModel.DataAnnotations;

namespace ReservaPassagens.Modelos
{
    public class Reserva
    {
        public int Id { get; set; }
        [Required]
        public string NumeroReserva { get; set; }
        public DateTime DataReserva { get; set; }
        public int VooId { get; set; }
        public Voo Voo { get; set; }
        public string CPFPassageiro { get; set; }
        public ICollection<Assento> Assentos { get; set; }
    }
}
