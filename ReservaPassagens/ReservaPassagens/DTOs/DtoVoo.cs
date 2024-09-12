using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;
using ReservaPassagens.Data;
namespace ReservaPassagens.DTOs
{
    public class DtoVoo
    {
        public int Id { get; set; }
        public string NumeroVoo { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime HoraPartida { get; set; }
        public DateTime HoraChegada { get; set; }
        public string CompanhiaAerea { get; set; }
        public decimal Preco { get; set; }
    }
}
