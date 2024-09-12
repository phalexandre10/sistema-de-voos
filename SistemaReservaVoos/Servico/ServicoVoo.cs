using Microsoft.EntityFrameworkCore;
using SistemaReservasVoos.Data;
using SistemaReservasVoos.Modelos;


namespace SistemaReservasVoos.Servico
{
    public class ServicoVoo
    {
        private readonly SistemaReservasContext _db;

        public ServicoVoo(SistemaReservasContext db)
        {
            _db = db;
        }

        public async Task<List<Voo>> ConsultarVoos(string origem, string destino, DateTime dataIda, DateTime? dataVolta = null)
        {
            var voosIda = await _db.Voos
                .Where(v => v.Origem == origem && v.Destino == destino && v.DataPartida.Date == dataIda.Date)
                .ToListAsync();

            if (dataVolta.HasValue)
            {
                var voosVolta = await _db.Voos
                    .Where(v => v.Origem == destino && v.Destino == origem && v.DataPartida.Date == dataVolta.Value.Date)
                    .ToListAsync();

                voosIda.AddRange(voosVolta);
            }

            return voosIda;
        }

        public async Task<Reserva> ReservarVoo(int vooId, string passageiroCpf, int quantidadePassageiros, FormaPagamento formaPagamento)
        {

            if (quantidadePassageiros <= 0)
                throw new ArgumentException("A quantidade de passageiros deve ser maior que zero.", nameof(quantidadePassageiros));

            var voo = await _db.Voos.FindAsync(vooId);
            if (voo == null)
                throw new InvalidOperationException("Voo não encontrado.");

            var reserva = new Reserva
            {
                CPFPassageiro = passageiroCpf,
                VooId = vooId,
                DataReserva = DateTime.Now,
                QuantidadePassageiros = quantidadePassageiros,
                CheckInRealizado = false,
                FormaPagamento = formaPagamento
            };

            _db.Reservas.Add(reserva);
            voo.AssentosDisponiveis -= quantidadePassageiros;
            await _db.SaveChangesAsync();
            return reserva;
        }

        public async Task CancelarReserva(int reservaId)
        {
            var reserva = await _db.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                throw new InvalidOperationException("Reserva não encontrada.");
            }

            var voo = await _db.Voos.FindAsync(reserva.VooId);
            if (voo == null)
            {
                throw new InvalidOperationException("Voo não encontrado.");
            }

            _db.Reservas.Remove(reserva);
            voo.AssentosDisponiveis += reserva.QuantidadePassageiros;
            await _db.SaveChangesAsync();
        }

        public async Task CancelarReservasNoShow()
        {
            var reservasParaCancelar = await _db.Reservas
                .Include(r => r.Voo)
                .Where(r => !r.CheckInRealizado && r.Voo.DataPartida <= DateTime.Now.AddHours(1))
                .ToListAsync();

            foreach (var reserva in reservasParaCancelar)
            {
                _db.Reservas.Remove(reserva);
                reserva.Voo.AssentosDisponiveis += reserva.QuantidadePassageiros;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> VerificarDisponibilidadeAssento(int vooId, string numeroAssento)
        {
            var voo = await _db.Voos.FindAsync(vooId);
            if (voo == null)
            {
                throw new InvalidOperationException("Voo não encontrado.");
            }

            var assentoOcupado = await _db.Reservas
                .AnyAsync(r => r.VooId == vooId && r.NumeroAssento == numeroAssento);

            return !assentoOcupado;
        }
    }
}
