# Sistema de Reservas e Voos


# Sobre o projeto
O código apresentado implementa um sistema de reservas de voos utilizando C# e ASP.NET Core. Este sistema oferece uma API várias  funcionalidades 


1. Gerenciamento de Passageiros: Permite cadastrar novos passageiros e atualizar informações existentes.

2. Consulta de Voos: Possibilita a busca de voos disponíveis com base em critérios como origem, destino e datas.

3. Reservas: Implementa a criação e cancelamento de reservas de voos.

4. Check-in: Oferece opções para realizar check-in online e presencial, com geração e envio de bilhetes eletrônicos.

5. Relatórios: Gera relatórios de ocupação semanal e vendas mensais.



## Layout swagger
![Mobile 1](https://github.com/acenelio/assets/raw/main/sds1/mobile1.png) ![Mobile 2](https://github.com/acenelio/assets/raw/main/sds1/mobile2.png)

## Layout web
![Web 1](https://github.com/acenelio/assets/raw/main/sds1/web1.png)

![Web 2](https://github.com/acenelio/assets/raw/main/sds1/web2.png)

## Digrama de caso de uso
![Modelo Conceitual](https://github.com/acenelio/assets/raw/main/sds1/modelo-conceitual.png)

## Estrutura do Projeto

- `Program.cs`: Configuração da aplicação e serviços
- `EndpointExtensions.cs`: Definição dos endpoints da API
- `ServicoPassageiro.cs`: Lógica de negócio para passageiros
- `ServicoVoo.cs`: Lógica de negócio para voos e reservas
- `ServicoCheckIn.cs`: Lógica de negócio para check-in
- `ServicoRelatorio.cs`: Geração de relatórios
- `CancelamentoReservasBackgroundService.cs`: Serviço em segundo plano para cancelar reservas não confirmadas

# Tecnologias utilizadas

- ASP.NET Core 8.0
- Entity Framework Core
- Swagger / OpenAPI
- SQL Server (pode ser alterado para outro banco suportado pelo EF Core)


#Como executar o projeto

## Configuração

1. Clone o repositório
2. Instale o .NET 8.0
3. Instalar as Extensões
4. Configure a string de conexão do banco de dados no `appsettings.json`:
json
{
"ConnectionStrings": {
"DefaultConnection": "Server=seu_servidor;Database=SistemaReservas;User Id=seu_usuario;Password=sua_senha;"
}
}
5. Execute as migrações do banco de dados:
Add-Migration
## Extensões Necessárias

Certifique-se de ter as seguintes extensões instaladas no seu projeto:

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design
- Swashbuckle.AspNetCore



# Autor

Wellington Mazoni de Andrade

https://www.linkedin.com/in/wmazoni
