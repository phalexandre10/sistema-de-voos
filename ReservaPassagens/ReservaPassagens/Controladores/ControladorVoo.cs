using Microsoft.AspNetCore.Mvc;
using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;
using ReservaPassagens.DTOs;
using ReservaPassagens.Data;
namespace ReservaPassagens.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControladorVoo : ControllerBase
    {
        private readonly IServicoVoo _servicoVoo;
        private readonly ILogger<ControladorVoo> _logger;

        public ControladorVoo(IServicoVoo servicoVoo, ILogger<ControladorVoo> logger)
        {
            _servicoVoo = servicoVoo;
            _logger = logger;
        }

        [HttpGet("consultar")]
        public async Task<ActionResult<ResultadoConsultaVoo>> ConsultarVoos([FromQuery] RequisicaoConsultaVoo requisicao)
        {
            try
            {
                var resultado = await _servicoVoo.ConsultarVoosAsync(requisicao);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar voos");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação." });
            }
        }
    }
}
