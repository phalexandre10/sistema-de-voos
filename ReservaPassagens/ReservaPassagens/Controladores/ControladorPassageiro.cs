using Microsoft.AspNetCore.Mvc;
using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;
using ReservaPassagens.DTOs;
namespace ReservaPassagens.Controladores
{

    // Controllers
    [ApiController]
    [Route("api/[controller]")]
    public class ControladorPassageiro : ControllerBase
    {
        private readonly IServicoPassageiro _servicoPassageiro;
        private readonly ILogger<ControladorPassageiro> _logger;

        public ControladorPassageiro(IServicoPassageiro servicoPassageiro, ILogger<ControladorPassageiro> logger)
        {
            _servicoPassageiro = servicoPassageiro;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<DtoPassageiro>> RegistrarPassageiro(RequisicaoRegistroPassageiro requisicao)
        {
            var resultado = await _servicoPassageiro.RegistrarPassageiroAsync(requisicao);
            return CreatedAtAction(nameof(ObterPassageiro), new { id = resultado.Id }, resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DtoPassageiro>> ObterPassageiro(int id)
        {
            var passageiro = await _servicoPassageiro.ObterPassageiroAsync(id);
            if (passageiro == null)
            {
                return NotFound();
            }
            return passageiro;
        }

        [HttpGet]
        public async Task<ActionResult<ResultadoPaginado<DtoPassageiro>>> ObterPassageiros([FromQuery] ParametrosPaginacao parametrosPaginacao)
        {
            var passageiros = await _servicoPassageiro.ObterPassageirosAsync(parametrosPaginacao);
            return Ok(passageiros);
        }
    }
}
