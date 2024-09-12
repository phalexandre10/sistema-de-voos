using SistemaReservasVoos.Data;
using SistemaReservasVoos.Modelos;
using SistemaReservasVoos.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SistemaReservasVoos.Servico
{

    // Serviço para gerenciar check-ins
    public class ServicoCheckIn
    {
        private readonly SistemaReservasContext _db;
        private readonly IEmailService _emailService;
        private readonly ServicoVoo _servicoVoo;

        public ServicoCheckIn(SistemaReservasContext db, IEmailService emailService, ServicoVoo servicoVoo)
        {
            _db = db;
            _emailService = emailService;
            _servicoVoo = servicoVoo;
        }

        public async Task<bool> RealizarCheckIn(int reservaId, string numeroAssento)
        {
            var reserva = await _db.Reservas.Include(r => r.Voo).Include(r => r.Passageiro).FirstOrDefaultAsync(r => r.Id == reservaId);
            if (reserva == null)
            {
                throw new InvalidOperationException("Reserva não encontrada.");
            }
           
            if (reserva.Voo.DataPartida < DateTime.Now)
                throw new InvalidOperationException("Não é possível realizar check-in para um voo que já partiu.");


            var agora = DateTime.Now;
            if (agora < reserva.Voo.DataPartida.AddHours(-24) || agora > reserva.Voo.DataPartida.AddHours(-1))
            {
                throw new InvalidOperationException("Check-in só pode ser realizado entre 24h e 1h antes do voo.");
            }

            if (!string.IsNullOrEmpty(reserva.NumeroAssento))
            {
                throw new InvalidOperationException("Check-in já realizado para esta reserva.");
            }

            var assentoDisponivel = await _servicoVoo.VerificarDisponibilidadeAssento(reserva.VooId, numeroAssento);
            if (!assentoDisponivel)
            {
                throw new InvalidOperationException("O assento selecionado não está disponível.");
            }

            reserva.CheckInRealizado = true;
            reserva.NumeroAssento = numeroAssento;
            await _db.SaveChangesAsync();

            var bilhete = GerarBilheteEletronico(reserva);
            await _emailService.EnviarEmailAsync(reserva.Passageiro.Email, "Bilhete Eletrônico", bilhete);

            return true;
        }

        public async Task<bool> RealizarCheckInPresencial(int reservaId, string numeroAssento, Passageiro dadosAtualizados = null)
        {
            var reserva = await _db.Reservas.Include(r => r.Voo).Include(r => r.Passageiro).FirstOrDefaultAsync(r => r.Id == reservaId);
            if (reserva == null)
            {
                throw new InvalidOperationException("Reserva não encontrada.");
            }

            if (!string.IsNullOrEmpty(reserva.NumeroAssento))
            {
                throw new InvalidOperationException("Check-in já realizado para esta reserva.");
            }

            reserva.CheckInRealizado = true;
            reserva.NumeroAssento = numeroAssento;

            if (dadosAtualizados != null)
            {
                _db.Entry(reserva.Passageiro).CurrentValues.SetValues(dadosAtualizados);
            }

            await _db.SaveChangesAsync();

            var bilhete = GerarBilheteEletronico(reserva);
            await _emailService.EnviarEmailAsync(reserva.Passageiro.Email, "Bilhete Eletrônico", bilhete);

            return true;
        }

        private string GerarBilheteEletronico(Reserva reserva)
        {
            return $@"
Bilhete Eletrônico
------------------
Voo: {reserva.Voo.Id}
Origem: {reserva.Voo.Origem}
Destino: {reserva.Voo.Destino}
Data de Partida: {reserva.Voo.DataPartida}

Companhia: {reserva.Voo.Companhia}
Assento: {reserva.NumeroAssento}
CPF do Passageiro: {reserva.CPFPassageiro}
Nome do Passageiro: {reserva.Passageiro.Nome}
Quantidade de Passageiros: {reserva.QuantidadePassageiros}
Data da Reserva: {reserva.DataReserva}
------------------
Este bilhete é válido para embarque. Apresente-o junto com um documento de identificação.
";
        }
    }
}
