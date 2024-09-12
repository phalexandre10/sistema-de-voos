namespace ReservaPassagens.DTOs
{
    public class DtoReserva
    {
        public int Id { get; set; }
        public string NumeroReserva { get; set; }
        public DateTime DataReserva { get; set; }
        public string NumeroVoo { get; set; }
        public List<string> NumerosAssentos { get; set; }
        public string CPFPassageiro { get; set; }
    }
}
