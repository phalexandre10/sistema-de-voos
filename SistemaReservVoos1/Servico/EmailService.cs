namespace SistemaReservasVoos.Servico
{
    public class EmailService : IEmailService
    {
        public Task EnviarEmailAsync(string destinatario, string assunto, string corpo)
        {
            Console.WriteLine($"E-mail enviado para {destinatario}. Assunto: {assunto}");


            DisponibilizarParaImpressao(corpo);

            return Task.CompletedTask;
        }

        private void DisponibilizarParaImpressao(string conteudo)
        {
            Console.WriteLine($"Bilhete disponível para impressão: {conteudo}");
        }
    }
}
