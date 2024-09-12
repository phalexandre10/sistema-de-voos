namespace ReservaPassagens.Serviço
{
    // Implement ServicoEmail
    public class ServicoEmail : IServicoEmail
    {
        private readonly ILogger<ServicoEmail> _logger;

        public ServicoEmail(ILogger<ServicoEmail> logger)
        {
            _logger = logger;
        }

        public async Task EnviarConfirmacaoReservaAsync(string email, DtoReserva reserva)
        {
            // Implement email sending logic for reservation confirmation
            _logger.LogInformation($"Enviando confirmação de reserva para {email}");
            await Task.CompletedTask;
        }

        public async Task EnviarBilheteAsync(string email, DtoBilhete bilhete)
        {
            // Implement email sending logic for ticket
            _logger.LogInformation($"Enviando bilhete eletrônico para {email}");
            // Here you would typically use an email service to send the actual email
            // For demonstration purposes, we're just logging the information
            _logger.LogInformation($"Bilhete: {bilhete.NumeroBilhete}, Voo: {bilhete.NumeroVoo}, Assento: {bilhete.NumeroAssento}");
            await Task.CompletedTask;
        }
    }

}
