using Microsoft.AspNetCore.Mvc;
using ReservaPassagens.Modelos;
using ReservaPassagens.Serviço;
using ReservaPassagens.Data;
using ReservaPassagens.DTOs;
namespace ReservaPassagens.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControladorRelatorios : ControllerBase
    {
        private readonly IServicoRelatorio _servicoRelatorio;
        private readonly IServicoRelatorioVendas _servicoRelatorioVendas;
        private readonly ILogger<ControladorRelatorios> _logger;

        public ControladorRelatorios(
            IServicoRelatorio servicoRelatorio,
            IServicoRelatorioVendas servicoRelatorioVendas,
            ILogger<ControladorRelatorios> logger)
        {
            _servicoRelatorio = servicoRelatorio;
            _servicoRelatorioVendas = servicoRelatorioVendas;
            _logger = logger;
        }

        [HttpGet("ocupacao")]
        public async Task<ActionResult<DtoRelatorioOcupacao>> GerarRelatorioOcupacao([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                var relatorio = await _servicoRelatorio.GerarRelatorioOcupacaoAsync(dataInicio, dataFim);
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de ocupação");
                return StatusCode(500, new { message = "Ocorreu um erro ao gerar o relatório de ocupação." });
            }
        }

        [HttpGet("vendas")]
        public async Task<ActionResult<DtoRelatorioVendas>> GerarRelatorioVendas([FromQuery] int mes, [FromQuery] int ano)
        {
            try
            {
                var relatorio = await _servicoRelatorioVendas.GerarRelatorioVendasAsync(mes, ano);
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de vendas");
                return StatusCode(500, new { message = "Ocorreu um erro ao gerar o relatório de vendas." });
            }
        }
    }

}
