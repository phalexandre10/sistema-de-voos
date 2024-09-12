using Microsoft.AspNetCore.Mvc;
using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;
using ReservaPassagens.Data;
using ReservaPassagens.DTOs;
namespace ReservaPassagens.Controladores
{

    [ApiController]
    [Route("api/[controller]")]
    public class ControladorReserva : ControllerBase
    {
        private readonly IServicoReserva _servicoReserva;
        private readonly ILogger<ControladorReserva> _logger;

        public ControladorReserva(IServicoReserva servicoReserva, ILogger<ControladorReserva> logger)
        {
            _servicoReserva = servicoReserva;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<DtoReserva>> RealizarReserva(RequisicaoReserva requisicao)
        {
            try
            {
                var reserva = await _servicoReserva.RealizarReservaAsync(requisicao);
                return CreatedAtAction(nameof(ObterReserva), new { id = reserva.Id }, reserva);
            }
            catch (AssentosIndisponiveisException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar reserva");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DtoReserva>> ObterReserva(int id)
        {
            var reserva = await _servicoReserva.ObterReservaAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return reserva;
        }
        [HttpPost("cancelar/{id}")]
        public async Task<ActionResult<DtoReserva>> CancelarReserva(int id)
        {
            try
            {
                var reservaCancelada = await _servicoReserva.CancelarReservaAsync(id);
                return Ok(reservaCancelada);
            }
            catch (ExcecaoReservaNaoEncontrada)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao cancelar reserva {id}");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }
        }

    }


}

