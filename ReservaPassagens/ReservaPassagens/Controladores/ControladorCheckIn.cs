using Microsoft.AspNetCore.Mvc;
using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;
using ReservaPassagens.Data;
using ReservaPassagens.DTOs;
namespace ReservaPassagens.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControladorCheckIn : ControllerBase
    {
        private readonly IServicoCheckIn _servicoCheckIn;
        private readonly ILogger<ControladorCheckIn> _logger;

        public ControladorCheckIn(IServicoCheckIn servicoCheckIn, ILogger<ControladorCheckIn> logger)
        {
            _servicoCheckIn = servicoCheckIn;
            _logger = logger;
        }

        [HttpPost("online")]
        public async Task<ActionResult<ResultadoCheckIn>> RealizarCheckInOnline(RequisicaoCheckInOnline requisicao)
        {
            try
            {
                var resultado = await _servicoCheckIn.RealizarCheckInOnlineAsync(requisicao);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar check-in online");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }
        }

        [HttpPost("presencial")]
        public async Task<ActionResult<ResultadoCheckIn>> RealizarCheckInPresencial(RequisicaoCheckInPresencial requisicao)
        {
            try
            {
                var resultado = await _servicoCheckIn.RealizarCheckInPresencialAsync(requisicao);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar check-in presencial");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }
        }
    }

}
