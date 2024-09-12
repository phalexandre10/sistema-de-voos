using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservaPassagens.Data;
using ReservaPassagens.Modelos;
namespace ReservaPassagens.Serviço
{

    public class ServicoReserva : IServicoReserva
    {
        private readonly ContextoAereo _contexto;
        private readonly IMapper _mapeador;
        private readonly ILogger<ServicoReserva> _logger;

        public ServicoReserva(ContextoAereo contexto, IMapper mapeador, ILogger<ServicoReserva> logger)
        {
            _contexto = contexto;
            _mapeador = mapeador;
            _logger = logger;
        }

        public async Task<DtoReserva> CancelarReservaAsync(int id)
        {
            var reserva = await _contexto.Reservas
                .Include(r => r.Voo)
                .ThenInclude(v => v.Assentos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                throw new ExcecaoReservaNaoEncontrada(id);
            }

            if (reserva.Status == StatusReserva.Cancelada)
            {
                throw new Exception("Esta reserva já está cancelada.");
            }

            reserva.Status = StatusReserva.Cancelada;

            // Liberar os assentos
            foreach (var assento in reserva.Voo.Assentos.Where(a => a.EstaOcupado))
            {
                assento.EstaOcupado = false;
            }

            await _contexto.SaveChangesAsync();
            _logger.LogInformation($"Reserva {id} cancelada com sucesso.");

            return _mapeador.Map<DtoReserva>(reserva);
        }

        public async Task CancelarReservasExpiradas()
        {
            var agora = DateTime.UtcNow;
            var reservasExpiradas = await _contexto.Reservas
                .Include(r => r.Voo)
                .ThenInclude(v => v.Assentos)
                .Where(r => r.Status == StatusReserva.Confirmada && r.Voo.HoraPartida <= agora.AddHours(1))
                .ToListAsync();

            foreach (var reserva in reservasExpiradas)
            {
                reserva.Status = StatusReserva.Cancelada;
                foreach (var assento in reserva.Voo.Assentos.Where(a => a.EstaOcupado))
                {
                    assento.EstaOcupado = false;
                }
                _logger.LogInformation($"Reserva {reserva.Id} cancelada automaticamente.");
            }

            await _contexto.SaveChangesAsync();
        }




        public async Task<DtoReserva> RealizarReservaAsync(RequisicaoReserva requisicao)
        {
            using var transaction = await _contexto.Database.BeginTransactionAsync();

            try
            {
                var voo = await _contexto.Voos
                    .Include(v => v.Assentos.Where(a => !a.EstaOcupado))
                    .FirstOrDefaultAsync(v => v.Id == requisicao.VooId);

                if (voo == null)
                {
                    throw new Exception("Voo não encontrado.");
                }

                if (voo.AssentosDisponiveis < requisicao.QuantidadePassageiros)
                {
                    throw new AssentosIndisponiveisException("Não há assentos suficientes disponíveis para esta reserva.");
                }

                var assentosReservados = voo.Assentos
                    .Where(a => !a.EstaOcupado)
                    .Take(requisicao.QuantidadePassageiros)
                    .ToList();

                var novaReserva = new Reserva
                {
                    NumeroReserva = GerarNumeroReserva(),
                    DataReserva = DateTime.UtcNow,
                    VooId = voo.Id,
                    CPFPassageiro = requisicao.CPFPassageiro,
                    Assentos = assentosReservados
                };

                _contexto.Reservas.Add(novaReserva);

                foreach (var assento in assentosReservados)
                {
                    assento.EstaOcupado = true;
                    assento.ReservaId = novaReserva.Id;
                }

                voo.AssentosDisponiveis -= requisicao.QuantidadePassageiros;

                await _contexto.SaveChangesAsync();
                await transaction.CommitAsync();

                return new DtoReserva
                {
                    Id = novaReserva.Id,
                    NumeroReserva = novaReserva.NumeroReserva,
                    DataReserva = novaReserva.DataReserva,
                    NumeroVoo = voo.NumeroVoo,
                    NumerosAssentos = assentosReservados.Select(a => a.NumeroAssento).ToList(),
                    CPFPassageiro = novaReserva.CPFPassageiro
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DtoReserva> ObterReservaAsync(int id)
        {
            var reserva = await _contexto.Reservas
                .Include(r => r.Voo)
                .Include(r => r.Assentos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                return null;
            }

            return new DtoReserva
            {
                Id = reserva.Id,
                NumeroReserva = reserva.NumeroReserva,
                DataReserva = reserva.DataReserva,
                NumeroVoo = reserva.Voo.NumeroVoo,
                NumerosAssentos = reserva.Assentos.Select(a => a.NumeroAssento).ToList(),
                CPFPassageiro = reserva.CPFPassageiro
            };
        }

        private string GerarNumeroReserva()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        public Task<DtoReserva> CriarReservaAsync(RequisicaoReserva requisicao)
        {
            throw new NotImplementedException();
        }
    }
}

