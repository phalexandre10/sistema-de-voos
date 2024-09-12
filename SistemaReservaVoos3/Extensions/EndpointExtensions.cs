using SistemaReservasVoos.DTO;
using SistemaReservasVoos.Modelos;
using SistemaReservasVoos.Servico;
namespace SistemaReservasVoos.Extensions
{

    public static class EndpointExtensions
    {
        public static void MapPassageiroEndpoints(this WebApplication app)
        {
            // Endpoint para cadastrar um novo passageiro
            app.MapPost("/passageiros", async (Passageiro passageiro, ServicoPassageiro servicoPassageiro) =>
            {
                try
                {
                    var novoPassageiro = await servicoPassageiro.ObterOuCadastrarPassageiro(passageiro.CPF, passageiro);
                    return Results.Created($"/passageiros/{novoPassageiro.CPF}", novoPassageiro);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // Endpoint para atualizar dados de um passageiro existente
            app.MapPut("/passageiros/{cpf}", async (string cpf, Passageiro passageiro, ServicoPassageiro servicoPassageiro) =>
            {
                if (cpf != passageiro.CPF)
                {
                    return Results.BadRequest("O CPF na URL não corresponde ao CPF do passageiro.");
                }

                try
                {
                    await servicoPassageiro.AtualizarDadosPassageiro(passageiro);
                    return Results.Ok("Dados do passageiro atualizados com sucesso.");
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            });
        }

        public static void MapVooEndpoints(this WebApplication app)
        {
            // Endpoint para consultar voos disponíveis
            app.MapGet("/voos", async (string origem, string destino, DateTime dataPartida, DateTime? dataRetorno, ServicoVoo servicoVoo) =>
            {
                if (string.IsNullOrEmpty(origem) || string.IsNullOrEmpty(destino))
                    return Results.BadRequest("Origem e Destino são obrigatórios.");

                var voos = await servicoVoo.ConsultarVoos(origem, destino, dataPartida, dataRetorno);
                var resultados = voos.Select(v => new
                {
                    v.Id,
                    v.Origem,
                    v.Destino,
                    v.DataPartida,
                    v.Horario,
                    v.Companhia,
                    v.Preco,
                    v.AssentosDisponiveis
                });
                return Results.Ok(resultados);
            });
        }

        public static void MapReservaEndpoints(this WebApplication app)
        {
            // Endpoint para criar uma nova reserva
            app.MapPost("/reservas", async (ReservaRequest request, ServicoVoo servicoVoo) =>
            {
                try
                {
                    var reserva = await servicoVoo.ReservarVoo(request.VooId, request.CPFPassageiro, request.QuantidadePassageiros, request.FormaPagamento);
                    return Results.Created($"/reservas/{reserva.Id}", reserva);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // Endpoint para cancelar uma reserva existente
            app.MapDelete("/reservas/{id}", async (int id, ServicoVoo servicoVoo) =>
            {
                try
                {
                    await servicoVoo.CancelarReserva(id);
                    return Results.Ok("Reserva cancelada com sucesso.");
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            });
        }

        public static void MapCheckInEndpoints(this WebApplication app)
        {
            // Endpoint para realizar check-in online
            app.MapPost("/check-in", async (CheckInRequest request, ServicoCheckIn servicoCheckIn) =>
            {
                try
                {
                    await servicoCheckIn.RealizarCheckIn(request.ReservaId, request.NumeroAssento);
                    return Results.Ok("Check-in realizado com sucesso. Bilhete eletrônico enviado por e-mail.");
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // Endpoint para realizar check-in presencial
            app.MapPost("/check-in-presencial", async (CheckInPresencialRequest request, ServicoCheckIn servicoCheckIn) =>
            {
                try
                {
                    await servicoCheckIn.RealizarCheckInPresencial(request.ReservaId, request.NumeroAssento, request.DadosAtualizados);
                    return Results.Ok("Check-in presencial realizado com sucesso. Bilhete eletrônico enviado por e-mail.");
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });
        }

        public static void MapRelatorioEndpoints(this WebApplication app)
        {
            // Endpoint para gerar relatório de ocupação semanal
            app.MapGet("/relatorio-ocupacao", async (ServicoRelatorio servicoRelatorio) =>
            {
                var relatorio = await servicoRelatorio.GerarRelatorioOcupacaoSemanal();
                return Results.Ok(relatorio);
            });

            // Endpoint para gerar relatório de vendas mensal
            app.MapGet("/relatorio-vendas", async (ServicoRelatorio servicoRelatorio) =>
            {
                var relatorio = await servicoRelatorio.GerarRelatorioVendasMensal();
                return Results.Ok(relatorio);
            });
        }
    }

}
