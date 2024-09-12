using System.ComponentModel.DataAnnotations;

namespace ReservaPassagens.Modelos
{
    public class Voo
    {
        public int Id { get; set; }
        [Required]
        public string NumeroVoo { get; set; }
        [Required]
        public string Origem { get; set; }
        [Required]
        public string Destino { get; set; }
        public DateTime HoraPartida { get; set; }
        public DateTime HoraChegada { get; set; }
        [Required]
        public string CompanhiaAerea { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Preco { get; set; }
        public ICollection<Assento> Assentos { get; set; }
        public int AssentosDisponiveis { get; set; }
    }
}
