using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;
using ReservaPassagens.Data;

namespace ReservaPassagens.DTOs;

public class DtoPassageiro
{
    public int Id { get; set; }
    public string CPF { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }


}
