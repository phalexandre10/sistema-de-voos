using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using AutoMapper;
using ReservaPassagens.Controladores;
using ReservaPassagens.Serviço;
using ReservaPassagens.Modelos;
using ReservaPassagens.Data;
using ReservaPassagens.DTOs;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Airline Reservation API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<ContextoAereo>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program));

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Configure JWT authentication options
    });

// Register services
builder.Services.AddScoped<IServicoPassageiro, ServicoPassageiro>();
builder.Services.AddScoped<IServicoVoo, ServicoVoo>();
builder.Services.AddScoped<IServicoReserva, ServicoReserva>();
builder.Services.AddScoped<IServicoBilhete, ServicoBilhete>();
builder.Services.AddScoped<IServicoEmail, ServicoEmail>();
builder.Services.AddScoped<IServicoRelatorio, ServicoRelatorio>();
builder.Services.AddScoped<IServicoRelatorioVendas, ServicoRelatorioVendas>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airline Reservation API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();


// DTOs and Request Models

//requisição

public class RequisicaoVerificacaoCPF
{
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos")]
    public string CPF { get; set; }
}


public class RequisicaoRegistroPassageiro
{
    [Required]
    public string CPF { get; set; }
    [Required]
    public string Nome { get; set; }
    [Required]
    public string Endereco { get; set; }
    [Required]
    public string TelefoneCelular { get; set; }
    public string TelefoneFixo { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

public class RequisicaoConsultaVoo
{
    [Required]
    public string Origem { get; set; }
    [Required]
    public string Destino { get; set; }
    [Required]
    public DateTime DataPartida { get; set; }
    public DateTime? DataRetorno { get; set; }
}

public class RequisicaoReserva
{
    [Required]
    public int VooId { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int QuantidadePassageiros { get; set; }
    [Required]
    public string CPFPassageiro { get; set; }
}



// New DTOs
public class RequisicaoCheckInOnline
{
    public int ReservaId { get; set; }
    public DadosPassageiro DadosPassageiro { get; set; }
    public string NumeroAssento { get; set; }
}

public class RequisicaoCheckInPresencial
{
    public int ReservaId { get; set; }
    public DadosPassageiro DadosPassageiro { get; set; }
    public string NumeroAssento { get; set; }
}




//classes

public class ResultadoConsultaVoo
{
    public List<DtoVoo> VoosIda { get; set; }
    public List<DtoVoo> VoosVolta { get; set; }
}




// Add ParametrosPaginacao class
public class ParametrosPaginacao
{
    public int NumeroPagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
}

// Add ResultadoPaginado class
public class ResultadoPaginado<T>
{
    public List<T> Itens { get; set; }
    public int ContagemTotal { get; set; }
    public int NumeroPagina { get; set; }
    public int TamanhoPagina { get; set; }
}






public class DadosPassageiro
{
    public string CPF { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string TelefoneCelular { get; set; }
}

public class ResultadoCheckIn
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
    public string NumeroAssento { get; set; }
}

// Add new DTOs

public class OcupacaoVoo
{
    public string NumeroVoo { get; set; }
    public DateTime DataPartida { get; set; }
    public double PorcentagemOcupacao { get; set; }
}

public class VendasPorVoo
{
    public string NumeroVoo { get; set; }
    public decimal TotalVendas { get; set; }
}

public class VendasPorFormaPagamento
{
    public FormaPagamento FormaPagamento { get; set; }
    public decimal TotalVendas { get; set; }
}

public enum FormaPagamento
{
    Cartao,
    Transferencia,
    Dinheiro
}




//exceção
// New custom exceptions
public class ExcecaoForaJanelaCheckIn : Exception
{
    public ExcecaoForaJanelaCheckIn()
        : base("O check-in só pode ser realizado entre 24 horas e 1 hora antes do voo.")
    {
    }
}

public class ExcecaoDadosPassageiroInvalidos : Exception
{
    public ExcecaoDadosPassageiroInvalidos()
        : base("Os dados do passageiro informados não correspondem aos dados da reserva.")
    {
    }
}

public class ExcecaoAssentoIndisponivel : Exception
{
    public ExcecaoAssentoIndisponivel(string numeroAssento)
        : base($"O assento {numeroAssento} não está disponível.")
    {
    }
}



public class AssentosIndisponiveisException : Exception
{
    public AssentosIndisponiveisException(string message) : base(message)
    {
    }
}


// New custom exception
public class ExcecaoBilheteNaoEncontrado : Exception
{
    public ExcecaoBilheteNaoEncontrado(int id)
        : base($"Bilhete com ID {id} não encontrado.")
    {
    }
}




// Custom Exceptions
public class ExcecaoPassageiroJaExiste : Exception
{
    public ExcecaoPassageiroJaExiste(string cpf)
        : base($"Passageiro com CPF {cpf} já existe.")
    {
    }
}
//DTO

public class DtoRelatorioVendas
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public List<VendasPorVoo> VendasPorVoo { get; set; }
    public decimal TotalVendas { get; set; }
    public List<VendasPorFormaPagamento> VendasPorFormaPagamento { get; set; }
    public double ComparacaoMesAnterior { get; set; }
}
public class DtoRelatorioOcupacao
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public List<OcupacaoVoo> OcupacaoPorVoo { get; set; }
}
