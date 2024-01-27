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
dotnet new sln

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

Interface para definir os métodos para interagir com o veículo

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

Configure contexto para acesso ao banco de dados

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

## API

### Conexão com o Banco de Dados

Defina no appsettings.json a string de conexão com o banco.
Obs: Essa configuração em produção deve ser sobrescrita na esteira por um Key Vault.

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

Adicione os pacotes ao projeto da API

```csharp
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Swashbuckle.AspNetCore
```

Habilite a documentação com notação de XML ao csproj

```xml
<PropertyGroup>
 <GenerateDocumentationFile>true</GenerateDocumentationFile>
 <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

Crie um método no BuilderExtension para adicionar o Swagger

```csharp
public static void AddSwagger(this WebApplicationBuilder builder)
{
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(options =>
  {
      options.SwaggerDoc("v1", new OpenApiInfo
      {
          Title = "FleetMGMT SincIdeia - API",
          Version = "v1",
          Description = "API com informações de produtos"
      });

      // Habilita comentários com notação XML
      var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
  });
}
```

Atribua a cada rota a respectiva documentação com anotações

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

Atualize a inicialização do App

```csharp
using FleetMGMT.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.AddServices();
builder.AddSwagger();

var app = builder.Build();
app.MapControllers();

// Carrega o Swagger para acesso em https://EnderecoDoMeuApp/swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
```

<!-- Links -->

[FleetMGMTV1]: assets/FleetMGMTv1.png
[Dapper]: https://balta.io/blog/dapper-crud
[SqlServer]: https://balta.io/blog/sql-server-docker
[CloudClusters]: https://www.cloudclusters.io/cloud/mssql/
[Azure]: https://azure.microsoft.com/pt-br/products/app-service
