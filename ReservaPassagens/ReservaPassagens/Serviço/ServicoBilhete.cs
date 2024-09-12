using ReservaPassagens.Data;

using ReservaPassagens.Modelos;
namespace ReservaPassagens.Serviço
{
    public class ServicoBilhete : IServicoBilhete
    {
        private readonly ContextoAereo _contexto;
        private readonly ILogger<ServicoBilhete> _logger;

        public ServicoBilhete(ContextoAereo contexto, ILogger<ServicoBilhete> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        public async Task<DtoBilhete> EmitirBilheteAsync(int reservaId)
        {
            var reserva = await _contexto.Reservas
                .Include(r => r.Passageiro)
                .Include(r => r.Voo)
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva == null)
            {
                throw new ExcecaoReservaNaoEncontrada(reservaId);
            }

            var bilhete = new Bilhete
            {
                ReservaId = reserva.Id,
                NumeroBilhete = GerarNumeroBilhete(),
                Preco = reserva.Voo.Preco
            };

            _contexto.Bilhetes.Add(bilhete);
            await _contexto.SaveChangesAsync();

            return new DtoBilhete
            {
                NumeroBilhete = bilhete.NumeroBilhete,
                NomePassageiro = reserva.Passageiro.Nome,
                Origem = reserva.Voo.Origem,
                Destino = reserva.Voo.Destino,
                DataHoraPartida = reserva.Voo.HoraPartida,
                NumeroVoo = reserva.Voo.NumeroVoo,
                NumeroAssento = await ObterNumeroAssentoAsync(reserva.Id),
                Preco = bilhete.Preco
            };
        }

        public async Task<DtoBilhete> ObterBilheteAsync(int id)
        {
            var bilhete = await _contexto.Bilhetes
                .Include(b => b.Reserva)
                .ThenInclude(r => r.Passageiro)
                .Include(b => b.Reserva)
                .ThenInclude(r => r.Voo)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bilhete == null)
            {
                throw new ExcecaoBilheteNaoEncontrado(id);
            }

            return new DtoBilhete
            {
                NumeroBilhete = bilhete.NumeroBilhete,
                NomePassageiro = bilhete.Reserva.Passageiro.Nome,
                Origem = bilhete.Reserva.Voo.Origem,
                Destino = bilhete.Reserva.Voo.Destino,
                DataHoraPartida = bilhete.Reserva.Voo.HoraPartida,
                NumeroVoo = bilhete.Reserva.Voo.NumeroVoo,
                NumeroAssento = await ObterNumeroAssentoAsync(bilhete.ReservaId),
                Preco = bilhete.Preco
            };
        }

        private string GerarNumeroBilhete()
        {
            // Implement a logic to generate a unique ticket number
            return $"TKT-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        private async Task<string> ObterNumeroAssentoAsync(int reservaId)
        {
            var assento = await _contexto.Assentos
                .FirstOrDefaultAsync(a => a.ReservaId == reservaId);
            return assento?.NumeroAssento ?? "N/A";
        }
    }

}
