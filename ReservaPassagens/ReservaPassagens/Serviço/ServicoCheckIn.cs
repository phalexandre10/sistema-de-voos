using ReservaPassagens.Data;
using ReservaPassagens.Modelos;

namespace ReservaPassagens.Serviço
{

    // Implement ServicoCheckIn
    public class ServicoCheckIn : IServicoCheckIn
    {
        private readonly ContextoAereo _contexto;
        private readonly ILogger<ServicoCheckIn> _logger;
        private readonly IServicoBilhete _servicoBilhete;
        private readonly IServicoEmail _servicoEmail;

        public ServicoCheckIn(ContextoAereo contexto, ILogger<ServicoCheckIn> logger,
                              IServicoBilhete servicoBilhete, IServicoEmail servicoEmail)
        {
            _contexto = contexto;
            _logger = logger;
            _servicoBilhete = servicoBilhete;
            _servicoEmail = servicoEmail;
        }
        public async Task<ResultadoCheckIn> RealizarCheckInOnlineAsync(RequisicaoCheckInOnline requisicao)
        {
            // ... existing check-in logic ...

            var resultado = new ResultadoCheckIn
            {
                Sucesso = true,
                Mensagem = "Check-in realizado com sucesso",
                NumeroAssento = assento.NumeroAssento
            };

            // Generate and send ticket
            await GerarEEnviarBilhete(reserva);

            return resultado;
        }

        public async Task<ResultadoCheckIn> RealizarCheckInPresencialAsync(RequisicaoCheckInPresencial requisicao)
        {
            // ... existing check-in logic ...

            var resultado = new ResultadoCheckIn
            {
                Sucesso = true,
                Mensagem = "Check-in presencial realizado com sucesso",
                NumeroAssento = assento.NumeroAssento
            };

            // Generate and send ticket
            await GerarEEnviarBilhete(reserva);

            return resultado;
        }

        private async Task GerarEEnviarBilhete(Reserva reserva)
        {
            var bilhete = await _servicoBilhete.EmitirBilheteAsync(reserva.Id);
            await _servicoEmail.EnviarBilheteAsync(reserva.Passageiro.Email, bilhete);
        }


        public async Task<ResultadoCheckIn> RealizarCheckInOnlineAsync(RequisicaoCheckInOnline requisicao)
        {
            var reserva = await _contexto.Reservas
                .Include(r => r.Voo)
                .Include(r => r.Passageiro)
                .FirstOrDefaultAsync(r => r.Id == requisicao.ReservaId);

            if (reserva == null)
            {
                throw new ExcecaoReservaNaoEncontrada(requisicao.ReservaId);
            }

            ValidarJanelaCheckIn(reserva.Voo.HoraPartida);
            ValidarDadosPassageiro(reserva.Passageiro, requisicao.DadosPassageiro);

            var assento = await AtribuirAssento(reserva.VooId, requisicao.NumeroAssento);

            reserva.Status = StatusReserva.CheckInRealizado;
            await _contexto.SaveChangesAsync();

            return new ResultadoCheckIn
            {
                Sucesso = true,
                Mensagem = "Check-in realizado com sucesso",
                NumeroAssento = assento.NumeroAssento
            };
        }

        public async Task<ResultadoCheckIn> RealizarCheckInPresencialAsync(RequisicaoCheckInPresencial requisicao)
        {
            var reserva = await _contexto.Reservas
                .Include(r => r.Voo)
                .Include(r => r.Passageiro)
                .FirstOrDefaultAsync(r => r.Id == requisicao.ReservaId);

            if (reserva == null)
            {
                throw new ExcecaoReservaNaoEncontrada(requisicao.ReservaId);
            }

            ValidarJanelaCheckIn(reserva.Voo.HoraPartida);

            // Update passenger data if necessary
            AtualizarDadosPassageiro(reserva.Passageiro, requisicao.DadosPassageiro);

            var assento = await AtribuirAssento(reserva.VooId, requisicao.NumeroAssento);

            reserva.Status = StatusReserva.CheckInRealizado;
            await _contexto.SaveChangesAsync();

            return new ResultadoCheckIn
            {
                Sucesso = true,
                Mensagem = "Check-in presencial realizado com sucesso",
                NumeroAssento = assento.NumeroAssento
            };
        }

        private void ValidarJanelaCheckIn(DateTime horaPartida)
        {
            var agora = DateTime.UtcNow;
            var janelaMinimaCheckIn = horaPartida.AddHours(-24);
            var janelaMaximaCheckIn = horaPartida.AddHours(-1);

            if (agora < janelaMinimaCheckIn || agora > janelaMaximaCheckIn)
            {
                throw new ExcecaoForaJanelaCheckIn();
            }
        }

        private void ValidarDadosPassageiro(Passageiro passageiro, DadosPassageiro dadosInformados)
        {
            if (passageiro.CPF != dadosInformados.CPF || passageiro.Nome != dadosInformados.Nome)
            {
                throw new ExcecaoDadosPassageiroInvalidos();
            }
        }

        private void AtualizarDadosPassageiro(Passageiro passageiro, DadosPassageiro novosDados)
        {
            passageiro.Nome = novosDados.Nome;
            passageiro.Email = novosDados.Email;
            passageiro.TelefoneCelular = novosDados.TelefoneCelular;
        }

        private async Task<Assento> AtribuirAssento(int vooId, string numeroAssento)
        {
            var assento = await _contexto.Assentos
                .FirstOrDefaultAsync(a => a.VooId == vooId && a.NumeroAssento == numeroAssento && !a.EstaOcupado);

            if (assento == null)
            {
                throw new ExcecaoAssentoIndisponivel(numeroAssento);
            }

            assento.EstaOcupado = true;
            return assento;
        }
    }

}
