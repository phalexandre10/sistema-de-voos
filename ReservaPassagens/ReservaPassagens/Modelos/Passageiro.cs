using System.ComponentModel.DataAnnotations;

namespace ReservaPassagens.Modelos
{

    public class Passageiro
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos")]
        public string CPF { get; set; }
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        [Phone]
        public string TelefoneCelular { get; set; }
        [Phone]
        public string TelefoneFixo { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
