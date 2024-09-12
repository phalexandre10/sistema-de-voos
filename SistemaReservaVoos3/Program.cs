using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using SistemaReservasVoos.Modelos;
using SistemaReservasVoos.Data;
using SistemaReservasVoos.Extensions;
using SistemaReservasVoos.Servico;
using Swashbuckle.AspNetCore.SwaggerGen;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Reservas de Voos",
        Version = "v1",
        Description = @"
API para gerenciamento de reservas de voos.

Endpoints dispon?veis:

1. Passageiros:
   - POST /passageiros: Cadastra um novo passageiro
   - PUT /passageiros/{cpf}: Atualiza os dados de um passageiro existente

2. Voos:
   - GET /voos: Consulta voos dispon?veis

3. Reservas:
   - POST /reservas: Cria uma nova reserva
   - DELETE /reservas/{id}: Cancela uma reserva existente

4. Check-in:
   - POST /check-in: Realiza check-in online
   - POST /check-in-presencial: Realiza check-in presencial

5. Relat?rios:
   - GET /relatorio-ocupacao: Gera relat?rio de ocupa??o semanal
   - GET /relatorio-vendas: Gera relat?rio de vendas mensal
"
    });


    // Passageiros
    c.SwaggerDoc("passageiros", new OpenApiInfo
    {
        Title = "Passageiros",
        Description = @"
Endpoints para gerenciamento de passageiros:

POST /passageiros
Descri??o: Cadastra um novo passageiro no sistema.
Par?metros:
- Corpo da requisi??o: Objeto Passageiro com os dados do novo passageiro
Respostas:
- 201: Passageiro cadastrado com sucesso
- 400: Dados inv?lidos ou incompletos

PUT /passageiros/{cpf}
Descri??o: Atualiza os dados de um passageiro existente.
Par?metros:
- cpf: CPF do passageiro (na URL)
- Corpo da requisi??o: Objeto Passageiro com os dados atualizados
Respostas:
- 200: Dados do passageiro atualizados com sucesso
- 400: Dados inv?lidos ou CPF n?o corresponde
- 404: Passageiro n?o encontrado
"
    });

    // Voos
    c.SwaggerDoc("voos", new OpenApiInfo
    {
        Title = "Voos",
        Description = @"
Endpoints para consulta de voos:

GET /voos
Descri??o: Consulta voos dispon?veis com base nos crit?rios fornecidos.
Par?metros:
- origem: Cidade de origem (string, obrigat?rio)
- destino: Cidade de destino (string, obrigat?rio)
- dataPartida: Data de partida (DateTime, obrigat?rio)
- dataRetorno: Data de retorno (DateTime, opcional)
Respostas:
- 200: Lista de voos dispon?veis
- 400: Par?metros inv?lidos ou ausentes
"
    });

    // Reservas
    c.SwaggerDoc("reservas", new OpenApiInfo
    {
        Title = "Reservas",
        Description = @"
Endpoints para gerenciamento de reservas:

POST /reservas
Descri??o: Cria uma nova reserva de voo.
Par?metros:
- Corpo da requisi??o: Objeto ReservaRequest com os detalhes da reserva
Respostas:
- 201: Reserva criada com sucesso
- 400: Dados inv?lidos ou voo indispon?vel

DELETE /reservas/{id}
Descri??o: Cancela uma reserva existente.
Par?metros:
- id: ID da reserva (int, na URL)
Respostas:
- 200: Reserva cancelada com sucesso
- 404: Reserva n?o encontrada
"
    });

    // Check-in
    c.SwaggerDoc("check-in", new OpenApiInfo
    {
        Title = "Check-in",
        Description = @"
Endpoints para realiza??o de check-in:

POST /check-in
Descri??o: Realiza check-in online para uma reserva.
Par?metros:
- Corpo da requisi??o: Objeto CheckInRequest com os detalhes do check-in
Respostas:
- 200: Check-in realizado com sucesso
- 400: Dados inv?lidos ou check-in n?o permitido

POST /check-in-presencial
Descri??o: Realiza check-in presencial para uma reserva.
Par?metros:
- Corpo da requisi??o: Objeto CheckInPresencialRequest com os detalhes do check-in
Respostas:
- 200: Check-in presencial realizado com sucesso
- 400: Dados inv?lidos ou check-in n?o permitido
"
    });

    // Relat?rios
    c.SwaggerDoc("relatorios", new OpenApiInfo
    {
        Title = "Relat?rios",
        Description = @"
Endpoints para gera??o de relat?rios:

GET /relatorio-ocupacao
Descri??o: Gera um relat?rio de ocupa??o semanal dos voos.
Respostas:
- 200: Relat?rio de ocupa??o gerado com sucesso

GET /relatorio-vendas
Descri??o: Gera um relat?rio de vendas mensal.
Respostas:
- 200: Relat?rio de vendas gerado com sucesso
"
    });
});

builder.Services.AddDbContext<SistemaReservasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<ServicoPassageiro>();
builder.Services.AddScoped<ServicoVoo>();
builder.Services.AddScoped<ServicoCheckIn>();
builder.Services.AddScoped<ServicoRelatorio>();
builder.Services.AddHostedService<CancelamentoReservasBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<SistemaReservasContext>();
    dbContext.Database.Migrate();
}

app.MapPassageiroEndpoints();
app.MapVooEndpoints();
app.MapReservaEndpoints();
app.MapCheckInEndpoints();
app.MapRelatorioEndpoints();

app.Run();


public interface IEmailService
{
    Task EnviarEmailAsync(string destinatario, string assunto, string corpo);
}


// Enumera??o para as formas de pagamento
public enum FormaPagamento
{
    Cartao,
    Transferencia,
    Dinheiro
}

