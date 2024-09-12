using System.ComponentModel.DataAnnotations;

namespace SistemaReservasVoos.Modelos
{
    public class Passageiro
    {
        [Key]
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Celular { get; set; }
        public string TelefoneFixo { get; set; }
        public string Email { get; set; }
    }
}
