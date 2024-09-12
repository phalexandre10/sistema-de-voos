using ReservaPassagens.Modelos;
using System.ComponentModel.DataAnnotations;

namespace ReservaPassagens.Modelos
{
    public class Assento
    {
        public int Id { get; set; }
        [Required]
        public string NumeroAssento { get; set; }
        public bool EstaOcupado { get; set; }
        public int VooId { get; set; }
        public Voo Voo { get; set; }
        public int? ReservaId { get; set; }
        public Reserva Reserva { get; set; }
    }
}
