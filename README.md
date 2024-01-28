# Fleet Management

Olá Dev 🤓!

Esse projeto simula o cadastro básico de veículos com dados de locação.

Será elaborado apenas um CRUD. Detalhes como autenticação e autorização não estão no escopo.

## Detalhes do projeto:

Criar uma API utilizando .NET (core 3, 5, 6, 7 ou 8), que tenha 5 métodos:
Cadastro de veículo
Consulta de veículo
Exclusão de veículo
Atualização de veículo
Listagem de veículos

Criar uma aplicação web MVC que se comunique com essa API e ofereça opções para interagir com esses métodos da API.

Requisitos:
SQL Server para o banco de dados
Usar Stored Procedures para as operações com banco de dados
C# como linguagem de programação

Data limite: 04/02/2024

## Contexto

![FleetMGMT][FleetMGMTv1]

Foco na entrega de um projeto bem estruturado e funcional.

Codificação realizada em inglês e comentários em português. Definida linguagem Ubiquá
para termos específicos.

### Funcionalidades Base

- Cadastro de veículo
- Consulta de veículo
- Exclusão de veículo
- Atualização de veículo
- Listagem de veículos

### Notas da Versão

V1.0 - Versão inicial

## Arquitetura

DDD - Domain Driven Design

Seguindo uma arquitetura limpa e enxuta, dirigida por domínio, vamos simplificar o projeto:

**Estrutura:**

- **FleetMGMT**
  - `FleetMGMT.Core`
  - `FleetMGMT.Infra`
  - `FleetMGMT.Api`
  - `FleetMGMT.UI`

Resumo:

- **FleetMGMT.Core** -> Definição das regras de negócio com classes modeladas para organizar todas as informações referentes ao FleetMGMT.
  Aqui serão criadas Configurações, Entidades, VOs e Use Cases.

- **FleetMGMT.Infra** -> Contexto do banco de dados, refletindo na aplicação a estrutura de campos e tabelas.
  Aqui configuramos o Repositório para armazenar os dados.

- **FleetMGMT.Api** -> Projeto principal com a aplicação web.
  Aqui ficam os Endpoints com as rotas de acesso para cada tipo de informação.

- **FleetMGMT.UI** -> Projeto MVC Web, atuando como Frontend.
  Aqui ficam as telas para interação do usuário.

## Recursos

Pacotes e ferramentas utilizadas:

- [Dapper][Dapper] -> Micro ORM para facilitar o acesso a dados.

- [SQL Server 2019][SqlServer] -> Banco de dados padrão.

- [Cloud Clusters][CloudClusters] -> Nuvem para hospedar o banco de dados.

- [Azure App Services][Azure] -> Nuvem para hospedar as aplicações.

## Passo a passo

Crie o projeto, a solução e adicione as referências:

```csharp
// Cria um novo diretório e acessa o mesmo
mkdir FleetMGMT
cd .\FleetMGMT

mkdir src
cd .\src

// Cria uma nova solução para referenciar os projetos e bibliotecas
dotnet new sln --name FleetMGMT

// Cria cada tipo de projeto
dotnet new classlib -o FleetMGMT.Core
dotnet new classlib -o FleetMGMT.Infra
dotnet new web -o FleetMGMT.Api // Projeto principal com executável
dotnet new mvc -o FleetMGMT.UI // Projeto com a interface do usuário

// Adiciona os projetos a solução
dotnet sln add .\FleetMGMT.Core
dotnet sln add .\FleetMGMT.Infra
dotnet sln add .\FleetMGMT.Api
dotnet sln add .\FleetMGMT.UI

// Referencia os projetos para utilização dos modelos e configurações
cd .\FleetMGMT.Infra
dotnet add reference ..\FleetMGMT.Core\

cd ..
cd .\FleetMGMT.Api\
dotnet add reference ..\FleetMGMT.Core\
dotnet add reference ..\FleetMGMT.Infra\

cd ..
cd .\FleetMGMT.UI\
dotnet add reference ..\FleetMGMT.Core\
```

## Core

Crie uma classe estática **Configuration** para organizar todas as informações de serviços externos.

```csharp
namespace FleetMGMT.Core;
public static class Configuration
{
   public static DatabaseConfiguration Database { get; set; } = new();

   public class DatabaseConfiguration
   {
       public string ConnectionString { get; set; } = string.Empty;
   }
}
```

Crie os contextos. Esse padrão facilita muito a organização do código.

### SharedContext

Aqui ficam os recursos compartilhados pelos objetos da aplicação.

Ex: Entidade base para trabalhar com IDs, métodos de extensão utilitários, retorno padrão conforme caso de uso.

```csharp
// Entidade base para compartilhar propriedades padrão
namespace FleetMGMT.Core.Contexts.SharedContext.Entities;

// IEquatable permite comparar Guid's
public abstract class Entity : IEquatable<Guid>
{
    // Geração de ID's únicos e aleatórios
    protected Entity() => Id = Guid.NewGuid();
    public Guid Id { get; set; }
    public bool Equals(Guid id) => Id == id;
    public override int GetHashCode() => Id.GetHashCode();
}
```

### Modelagem

Entidade para representar um veículo com dados adicionais sobre locação.

```csharp
using FleetMGMT.Core.Contexts.SharedContext.Entities;

namespace FleetMGMT.Core.Contexts.VehicleContext.Entities;
public class Vehicle : Entity
{
    public string Name { get; set; } = null!; // Nome
    public string Brand { get; set; } = null!; // Marca
    public decimal MarketPrice { get; set; } // Valor de Mercado do Veículo
    public DateTime LeaseStart { get; set; } // Início da locação
    public DateTime LeaseEnd { get; set; } // Fim da locação
    public string Renter { get; set; } = null!; // Locatário
}
```

Interface para definir os métodos para interagir com o veículo.

```csharp
namespace FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
public interface IRepository
{
    Task<IEnumerable<Vehicle?>> GetAllAsync(); // Retorna lista com todos
    Task<Vehicle?> GetByIdAsync(Guid id); // Retorna apenas 1 com base no Id
    Task<int> CreateAsync(Vehicle model); // Cria um novo registro
    Task<int> UpdateAsync(Guid id, Vehicle model); // Atualiza um registro
    Task<int> DeleteAsync(Guid id); // Apaga o registro
}
```

## Infra

Mudando para o projeto de Infraestrutura, é hora de configurar os mapeamentos (DE/PARA) dos objetos da aplicação para o banco de dados.

```csharp
// Usando Dapper para persistência dos dados
cd .\src\FleetMGMT.Infra\
dotnet add package Microsoft.Data.SqlClient
dotnet add package Dapper
```

Configure o contexto para acesso ao banco de dados.

```csharp
using System.Data;
using Microsoft.Data.SqlClient;

namespace FleetMGMT.Infra.Contexts.DataContext;
public class AppDbContext(IDbConnection connection) : IDisposable
{
    // A conexão será injetada ao inicializar a aplicação
    private readonly IDbConnection _connection = connection
        ?? throw new ArgumentNullException(nameof(connection));

    public SqlConnection Connection
    {
        get
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            return _connection as SqlConnection ??
                throw new InvalidOperationException(@"Falha ao converter IDbConnection
                 para SqlConnection.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
                _connection.Close();
        }
    }
}
```

Agora, implemente a interface com um repositório para interagir com os dados.

```csharp
using System.Data;
using Dapper;
using FleetMGMT.Core.Contexts.VehicleContext.Entities;
using FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
using FleetMGMT.Infra.Contexts.DataContext;

namespace FleetMGMT.Infra.Contexts.VehicleContext.UseCases.CRUD;
public class VehicleRepository(AppDbContext context) : IRepository
{
    private readonly AppDbContext _context = context;

    // Retorna uma lista de veículos
    public async Task<IEnumerable<Vehicle?>> GetAllAsync()
    {
        return await _context
            .Connection
            .QueryAsync<Vehicle>(
            "spGetAllVehicles", null,
            commandType: CommandType.StoredProcedure);
    }

    // Retorna um veículo com base no Id
    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        return await _context
            .Connection
            .QuerySingleOrDefaultAsync<Vehicle>(
                "spGetVehicleById", parameters,
                commandType: CommandType.StoredProcedure);
    }

    // Demais métodos...
}
```

Execute os [Scripts SQL][SqlScripts] no Data Studio para gerar a base, procedimentos e realizar testes.

```sql
-- Cria o Banco
CREATE DATABASE FleetMGMTDB
COLLATE SQL_Latin1_General_CP1_CI_AI;

-- Cria a tabela de Veículos
USE FleetMGMTDB;
CREATE TABLE Vehicle
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(150) NOT NULL,
    [Brand] NVARCHAR(150),
    [MarketPrice] DECIMAL(18, 2),
    [LeaseStart] DATETIME NOT NULL,
    [LeaseEnd] DATETIME NOT NULL,
    [Renter] NVARCHAR(255) NOT NULL
);

-- Cria a procedure para retornar todos os veículos
CREATE PROCEDURE spGetAllVehicles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
    [Id],
    [Name],
    [Brand],
    [MarketPrice],
    [LeaseStart],
    [LeaseEnd],
    [Renter]
    FROM Vehicle;
END;

-- Cria procedure para retornar o veículo por Id
CREATE PROCEDURE spGetVehicleById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
    [Id],
    [Name],
    [Brand],
    [MarketPrice],
    [LeaseStart],
    [LeaseEnd],
    [Renter]
    FROM Vehicle
    WHERE Id = @Id;
END;

-- Procedure para inserir um veiculo
CREATE PROCEDURE spCreateVehicle
    @Name NVARCHAR(150),
    @Brand NVARCHAR(150),
    @MarketPrice DECIMAL(18,2),
    @LeaseStart DATE,
    @LeaseEnd DATE,
    @Renter NVARCHAR(255)
AS
BEGIN
    INSERT INTO [Vehicle]
    (
        [Id],
        [Name],
        [Brand],
        [MarketPrice],
        [LeaseStart],
        [LeaseEnd],
        [Renter]
    )
    VALUES
    (
        NEWID(),
        @Name,
        @Brand,
        @MarketPrice,
        @LeaseStart,
        @LeaseEnd,
        @Renter
    );
END
GO
```

## API

### Conexão com o Banco de Dados

Defina no appsettings.json a string de conexão com o banco.

Obs: Essa configuração em produção deve ser sobrescrita na esteira de sua [nuvem][AzureConnectionString].

```csharp
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FleetMGMTDB;User ID=sa;Password=MyPass;Trusted_Connection=False; TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Controllers

Crie os controllers definindo as rotas (endpoints) de acesso.

```csharp
using FleetMGMT.Core.Contexts.VehicleContext.Entities;
using FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FleetMGMT.Api.Controllers;
public class VehicleController(IRepository repository) : ControllerBase
{
    private readonly IRepository _repository = repository;

    [HttpGet]
    [Route("v1/api/vehicles")]
    public async Task<IEnumerable<Vehicle?>> Get() => await _repository.GetAllAsync();

    [HttpGet]
    [Route("v1/api/vehicles/{id}")]
    public async Task<Vehicle?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);

    // Demais endpoints...
}
```

Agora na inicialização da API, altere o builder para recuperar as informações.

```csharp
// Program.cs adicionando as configurações padrão e banco de dados
using FleetMGMT.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Métodos de extensão para deixar o builder limpo e organizado
builder.AddConfiguration();
builder.AddServices();

var app = builder.Build();
app.MapControllers();

app.Run();
```

## Documentando a API

Adicione os pacotes da OpenAPI ao projeto da API.

```csharp
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Swashbuckle.AspNetCore
```

Habilite a documentação com notação de XML ao csproj.

```xml
<PropertyGroup>
 <GenerateDocumentationFile>true</GenerateDocumentationFile>
 <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

Crie um método no BuilderExtension para adicionar o Swagger.

```csharp
public static void AddSwagger(this WebApplicationBuilder builder)
{
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(options =>
  {
      options.SwaggerDoc("v1", new OpenApiInfo
      {
          Title = "FleetMGMT - API",
          Version = "v1",
          Description = "API com dados de locação de veículos"
      });

      var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
  });
}
```

Atribua a cada rota a respectiva documentação com anotações.

```csharp
using FleetMGMT.Core.Contexts.VehicleContext.Entities;
using FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FleetMGMT.Api.Controllers;
public class VehicleController(IRepository repository) : ControllerBase
{
    private readonly IRepository _repository = repository;

    /// <summary>
    /// Obtém lista com todos os veículos.
    /// </summary>
    public async Task<IEnumerable<Vehicle?>> Get() => await _repository.GetAllAsync();

    // Demais rotas...
}
```

Atualize a inicialização do App.

```csharp
using FleetMGMT.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.AddServices();
builder.AddSwagger();

var app = builder.Build();
app.MapControllers();

// Carrega o Swagger para acesso em https://url-para-acessar-o-app/swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
```

Exemplo da documentação:

![SwaggerDemo][SwaggerDemo]

## UI - Interface do Usuário

Crie uma classe Configuration para recuperar a Url com o caminho de acesso a API.

```csharp
namespace FleetMGMT.UI
{
  public static class Configuration
  {
    public static ApiConfiguration Api { get; set; } = new();

    public class ApiConfiguration
    {
      public string BaseUrl { get; set; } = string.Empty;
    }
  }
}
```

Defina a Url no AppSettings.json.

```json
{
  "Secrets": {
    "ApiBaseUrl": "https://url-para-acessar-a-api"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Agora crie um BuilderExtension para adicionar essa configuração ao executar a aplicação Web.

```csharp
using FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD;

namespace FleetMGMT.UI.Extensions;
public static class BuilderExtension
{
  public static void AddConfiguration(this WebApplicationBuilder builder)
  {
    Configuration.Api.BaseUrl =
               builder.Configuration.GetSection("Secrets")
               .GetValue<string>("ApiBaseUrl") ?? string.Empty;
  }
}
```

## UI - Contextos

Seguindo a organização similar a API, crie contextos para as ViewModels e Controllers.
Obs: Por convenção, as paginas devem ficar dentro da estrutura de arquivos, na pasta Views.

Criando a ViewModel. A ViewModel pode ser diferente da model do projeto Core, pois nem todos os campos são necessários para exibição em tela. Assim temos flexibilidade para representar os dados para o usuário final.

```csharp
namespace FleetMGMT.UI.Contexts.VehicleContext.Models;
public class VehicleViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public decimal MarketPrice { get; set; }
    public DateTime LeaseStart { get; set; }
    public DateTime LeaseEnd { get; set; }
    public string Renter { get; set; } = null!;
}
```

Criando uma interface para definir quais serão os métodos de acesso a fonte de dados. Procure utilizar a convenção async quando possível. Métodos assíncronos tem a vantagem de não travar a tela do usuário enquanto é realizado o processamento.

```csharp
using FleetMGMT.UI.Contexts.VehicleContext.Models;

namespace FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD.Contracts;
public interface IVehicleService
{
    Task<IEnumerable<VehicleViewModel>> GetAllVehiclesAsync();
    Task<VehicleViewModel> GetVehicleByIdAsync(Guid id);
    Task CreateVehicleAsync(VehicleViewModel vehicle);
    Task UpdateVehicleAsync(Guid id, VehicleViewModel vehicle);
    Task DeleteVehicleAsync(Guid id);
}
```

Agora é a hora de criar o serviço de acesso a dados, implementando a interface.

```csharp
using System.Text;
using System.Text.Json;
using FleetMGMT.UI.Contexts.VehicleContext.Models;
using FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD.Contracts;

namespace FleetMGMT.UI.Contexts.VehicleContext.UseCases.CRUD;
public class VehicleService(HttpClient httpClient) : IVehicleService
{
    // O cliente Http é injetado ao iniciar a aplicação.
    private readonly HttpClient _httpClient = httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IEnumerable<VehicleViewModel>> GetAllVehiclesAsync()
    {
        // Definida a fonte de dados para consumo
        var response = await _httpClient.GetAsync("/v1/api/vehicles");
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();

        var vehicles = await JsonSerializer
            .DeserializeAsync<IEnumerable<VehicleViewModel>>(stream, _jsonOptions);

        // Caso não haja dados, retorna uma lista vazia
        return vehicles ?? [];
    }

    // Demais métodos...
}
```

Para finalizar, crie as Views (Páginas) para interação do usuário

Index - Listando as informações

```csharp
@{
  ViewData["Title"] = "Gestão de Frota";
}

<div class="text-center">
  <div>
    <h1 class="display-4">Gestão de Frota</h1>
    <a href="/Vehicles/Create" class="btn btn-primary">Novo</a>
  </div>
</div>

@if (Model != null)
{
  <div>
    <table class="table">
      <thead>
        <tr>
          <th scope="col">#</th>
          <th scope="col">Veículo</th>
          <th scope="col">Marca</th>
          <th scope="col">Valor de Mercado</th>
          <th scope="col">Início locação</th>
          <th scope="col">Fim locação</th>
          <th scope="col">Locatário</th>
          <th scope="col">Ações</th>
        </tr>
      </thead>
      <tbody>
        @{
          int rowId = 1;
          foreach (var vehicle in Model)
          {
            <tr>
              <td><a asp-action="Details" asp-route-id="@vehicle.Id">@rowId</a></td>
              <td>@vehicle.Name</td>
              <td>@vehicle.Brand</td>
              <td>
                @vehicle.MarketPrice.ToString("C", new System.Globalization.CultureInfo("pt-BR"))
              </td>
              <td>@vehicle.LeaseStart.ToString("dd/MM/yyyy")</td>
              <td>@vehicle.LeaseEnd.ToString("dd/MM/yyyy")</td>
              <td>@vehicle.Renter.ToUpper()</td>
              <td class="custom-actions">
                <a asp-action="Edit" asp-route-id="@vehicle.Id" style="text-decoration: none;" title="Editar">
                  📋
                </a>
                <a href="#" class="delete-link" data-toggle="modal" data-target="#deleteModal" data-vehicle-id="@vehicle.Id"
                  title="Excluir">
                  ❌
                </a>
              </td>
            </tr>
            rowId++;
          }
        }
      </tbody>
    </table>
  </div>
}
else
{
  <p>Não há dados para exibição</p>
}

<!-- Modal de Confirmação de Exclusão -->
<div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel"
  aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered" role="document">
    <div class="modal-content">

      <div class="modal-header">
        <h5 class="modal-title text-centered" id="deleteModalLabel">Confirmação de Exclusão</h5>
        <button type="button" class="btn btn-secondary close-modal ml-auto" data-dismiss="modal" aria-label="Fechar">
          <span aria-hidden="true" id="btn-close-x">x</span>
        </button>
      </div>

      <div class="modal-body text-center">
        Realmente quer excluir este registro?
        <input type="hidden" id="vehicleId" />
      </div>

      <div class="modal-footer justify-content-center">
        <button type="button" class="btn btn-secondary close-modal" data-dismiss="modal" id="cancelDelete">
          Cancelar
        </button>
        <button type="button" class="btn btn-danger" id="confirmDelete">
          Excluir
        </button>
      </div>
    </div>
  </div>
</div>

<script src="~/lib/jquery/dist/jquery.min.js"></script>
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>

<script>
  $(document).ready(function () {
    $('.delete-link').click(function (e) {
      e.preventDefault();
      var vehicleId = $(this).data('vehicle-id');
      var modal = $('#deleteModal');
      modal.find('#vehicleId').val(vehicleId);
      modal.modal('show');
    });

    $('#confirmDelete').click(function () {
      var vehicleId = $('#vehicleId').val();
      $.ajax({
        url: '/Vehicles/Delete/' + vehicleId,
        type: 'DELETE',
        success: function () {
          location.reload();
        }
      });
    });

    $('.close-modal').click(function () {
      $('#deleteModal').modal('hide');
    });
  });
</script>
```

Create - Cria um novo registro

```csharp
@model VehicleViewModel
@{
  ViewData["Title"] = "Nova Locação";
}

@if (Model != null)
{
  <div class="text-center">

    <h1 class="display-4">Nova locação</h1>

    <form class="form" method="post" asp-action="Create">
      <div class="mb-3">
        <label for="Name">Veículo</label>
        <input type="text" class="form-control" asp-for="Name">
      </div>

      <div class="mb-3">
        <label for="Brand">Marca</label>
        <input type="text" class="form-control" asp-for="Brand">
      </div>

      <div class="mb-3">
        <label for="MarketPrice">Valor de Mercado</label>
        <input type="text" class="form-control" asp-for="MarketPrice">
      </div>

      <div class="mb-3">
        <label for="LeaseStart">Início locação</label>
        <input type="date" class="form-control" asp-for="LeaseStart">
      </div>

      <div class="mb-3">
        <label for="LeaseEnd">Fim locação</label>
        <input type="date" class="form-control" asp-for="LeaseEnd">
      </div>

      <div class="mb-3">
        <label for="Renter">Locatário</label>
        <input type="text" class="form-control" asp-for="Renter">
      </div>

      <button class="btn btn-primary">Salvar</button>
    </form>
  </div>
}
else
{
  <p>O modelo não foi inicializado.</p>
}
```

Edit - Atualiza um registro

```csharp
@model VehicleViewModel
@{
  ViewData["Title"] = "Edita locação";
}

@if (Model != null)
{
  <div class="text-center">

    <h1 class="display-4">Edita locação</h1>

    <form method="post" asp-action="Edit">
      <input type="hidden" asp-for="Id" />

      <div class="mb-3">
        <label for="Name">Veículo</label>
        <input type="text" class="form-control" asp-for="Name">
      </div>

      <div class="mb-3">
        <label for="Brand">Marca</label>
        <input type="text" class="form-control" asp-for="Brand">
      </div>

      <div class="mb-3">
        <label for="MarketPrice">Valor de Mercado</label>
        <input type="text" class="form-control" asp-for="MarketPrice">
      </div>

      <div class="mb-3">
        <label for="LeaseStart">Início locação</label>
        <input type="date" class="form-control" asp-for="LeaseStart">
      </div>

      <div class="mb-3">
        <label for="LeaseEnd">Fim locação</label>
        <input type="date" class="form-control" asp-for="LeaseEnd">
      </div>

      <div class="mb-3">
        <label for="Renter">Locatário</label>
        <input type="text" class="form-control" asp-for="Renter">
      </div>

      <button class="btn btn-primary">Salvar</button>
    </form>
  </div>
}
else
{
  <p>O modelo não foi inicializado.</p>
}
```

Details - Consulta detalhes do registro

```csharp
@{
  ViewData["Title"] = "Detalhes Locação";
}

@if (Model != null)
{
  <div class="text-center">

    <h1 class="display-4">Detalhes da Locação</h1>

    <form>
      <div class="mb-3">
        <label for="Id">ID</label>
        <input type="text" class="form-control" id="Id" value="@Model.Id" readonly>
      </div>

      <div class="mb-3">
        <label for="Name">Veículo</label>
        <input type="text" class="form-control" id="Name" value="@Model.Name" readonly>
      </div>

      <div class="mb-3">
        <label for="Brand">Marca</label>
        <input type="text" class="form-control" id="Brand" value="@Model.Brand" readonly>
      </div>

      <div class="mb-3">
        <label for="MarketPrice">Valor de Mercado</label>
        <input type="text" class="form-control" id="MarketPrice" value="@Model.MarketPrice.ToString("C")" readonly>
      </div>

      <div class="mb-3">
        <label for="LeaseStart">Início locação</label>
        <input type="date" class="form-control" id="LeaseStart" value="@Model.LeaseStart.ToString("yyyy-MM-dd")"
               readonly>
      </div>

      <div class="mb-3">
        <label for="LeaseEnd">Fim locação</label>
        <input type="date" class="form-control" id="LeaseEnd" value="@Model.LeaseEnd.ToString("yyyy-MM-dd")"
               readonly>
      </div>

      <div class="mb-3">
        <label for="Renter">Locatário</label>
        <input type="text" class="form-control" id="Renter" value="@Model.Renter.ToUpper()" readonly>
      </div>

      <a href="/Vehicles" class="btn btn-primary">Voltar</a>
    </form>
  </div>
}
else
{
  <p>O modelo não foi inicializado.</p>
}
```

Atualize a navegação alterando a pagina Views -> Shared -> \_Layout.cshtml

```csharp
<!DOCTYPE html>
<html lang="pt-br">

<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>@ViewData["Title"] - FleetMGMT.UI</title>
  <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
  <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
  <link rel="stylesheet" href="~/FleetMGMT.UI.styles.css" asp-append-version="true" />
</head>

<body>
  <header>
    <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light
      bg-white border-bottom box-shadow mb-3">
      <div class="container-fluid">
        <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">FleetMGMT</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse"
                aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>

        // Opções do Menu principal atualizadas
        <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
          <ul class="navbar-nav flex-grow-1">
            <li class="nav-item">
              <a class="nav-link text-dark" asp-area=""
                 asp-controller="Home" asp-action="Index">Home</a>
            </li>
            <li class="nav-item">
              <a class="nav-link text-dark" asp-area="" asp-controller="Vehicles"
                 asp-action="Index">Veículos</a>
            </li>
          </ul>
        </div>

      </div>
    </nav>
  </header>

  <div class="container">
    <main role="main" class="pb-3">
      @RenderBody()
    </main>
  </div>

  <footer class="border-top footer text-muted text-center">
    <div class="container">
      &copy; @DateTime.Now.Year - FleetMGMT
    </div>
  </footer>

  <script src="~/lib/jquery/dist/jquery.min.js"></script>
  <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
  <script src="~/js/site.js" asp-append-version="true"></script>
  @await RenderSectionAsync("Scripts", required: false)
</body>

</html>
```

Para subir a aplicação, configure a inicialização e realize testes.

```csharp
// Program.cs
using FleetMGMT.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.AddServices();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

Exemplo de tela principal(Views -> Home -> Index.cshtml):

![IndexDemoUI][IndexDemoUI]

<!-- Links -->

[FleetMGMTv1]: assets/img/FleetMGMTv1.png
[Dapper]: https://balta.io/blog/dapper-crud
[SqlServer]: https://balta.io/blog/sql-server-docker
[CloudClusters]: https://www.cloudclusters.io/cloud/mssql/
[Azure]: https://azure.microsoft.com/pt-br/products/app-service
[SqlScripts]: assets/sql/
[AzureConnectionString]: https://learn.microsoft.com/en-us/azure/app-service/configure-common?tabs=portal#configure-connection-strings
[SwaggerDemo]: assets/img/Swagger-demo.png
[IndexDemoUI]: assets/img/IndexUI-Demo.png
