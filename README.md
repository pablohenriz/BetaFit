# Beta Fit

Projeto institucional/demonstrativo de portfólio: uma loja fitness fictícia composta por um
**Website público** e um **Desktop administrativo**, ambos consumindo a mesma **Web API REST**,
construída com **Clean Architecture** em .NET 8.

> ⚠️ Este projeto **não é um e-commerce de produção**. Não há pagamento, checkout, pedidos,
> estoque real, gateway de pagamento, transportadora ou emissão fiscal. O objetivo é demonstrar
> organização arquitetural, separação de responsabilidades e boas práticas de engenharia.

A arquitetura em camadas e a separação de responsabilidades foram inspiradas no projeto
[SenacGames](https://github.com/pablohenriz/SenacGames-Project-FULL), sem reaproveitamento de código.

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Domain | C# puro (sem dependências externas) |
| Application | C#, FluentValidation |
| Infrastructure | Entity Framework Core 8, SQL Server |
| API | ASP.NET Core 8 Web API, Swagger/Swashbuckle |
| UI (Website) | ASP.NET Core 8 Razor Pages |
| Desktop | WPF (.NET 8), MVVM, CommunityToolkit.Mvvm |
| Testes | xUnit, FluentAssertions, NSubstitute |

## Arquitetura

```
CLIENTE WEB (BetaFit.UI)         DESKTOP ADMIN (BetaFit.Desktop)
        │                                  │
        └───────────────┬──────────────────┘
                         ▼
                  BetaFit.API (HTTP/REST)
                         ▼
                BetaFit.Application (casos de uso)
                         ▼
                  BetaFit.Domain (regras de negócio)
                         ▲
                BetaFit.Infrastructure (EF Core)
                         ▼
                     SQL Server
```

Nem o Website nem o Desktop acessam o banco de dados diretamente — ambos falam **exclusivamente**
com a `BetaFit.API` via HTTP. Apenas a `BetaFit.Infrastructure` conhece EF Core e SQL Server.

Veja detalhes de cada camada em [`ARCHITECTURE.md`](ARCHITECTURE.md) e o dicionário de
entidades/endpoints em [`DOCUMENTATION.md`](DOCUMENTATION.md).

## Estrutura do repositório

```
BetaFit/
├── BetaFit.Domain/            # Entities, Enums, Exceptions, Interfaces
├── BetaFit.Application/       # DTOs, Services, Validators, Interfaces
├── BetaFit.Infrastructure/    # DbContext, Configurations, Repositories, Seed
├── BetaFit.API/               # Controllers, Middleware, Program.cs
├── BetaFit.UI/                # Website público (Razor Pages)
├── BetaFit.Desktop/           # Painel administrativo (WPF)
├── tests/
│   ├── BetaFit.Domain.Tests/
│   ├── BetaFit.Application.Tests/
│   └── BetaFit.API.Tests/
├── docs/
├── README.md
├── ARCHITECTURE.md
├── DOCUMENTATION.md
└── BetaFit.sln
```

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, Developer ou container Docker)
- Windows, se for executar o `BetaFit.Desktop` (WPF é uma tecnologia Windows-only)

## Como executar

### 1. Restaurar dependências

```bash
dotnet restore
```

### 2. Configurar a connection string

Edite `BetaFit.API/appsettings.json` (ou use `dotnet user-secrets`) com a connection string do
seu SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BetaFitDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3. Criar a migration inicial e o banco de dados

O projeto já está preparado para Migrations do EF Core, mas a migration inicial precisa ser
gerada localmente (arquivos de Migrations não versionados neste pacote):

```bash
dotnet tool install --global dotnet-ef   # se ainda não tiver a ferramenta
cd BetaFit.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../BetaFit.API
```

A própria `BetaFit.API` aplica as migrations e popula o **Seed** automaticamente na primeira
execução (`BetaFitDbSeeder.SeedAsync`, chamado em `Program.cs`).

### 4. Executar a API

```bash
cd BetaFit.API
dotnet run
```

Swagger disponível em `https://localhost:5001/swagger`.

### 5. Executar o Website

```bash
cd BetaFit.UI
dotnet run
```

Disponível em `https://localhost:5011`. Confirme que `BetaFitApi:BaseUrl` em
`BetaFit.UI/appsettings.json` aponta para a URL da API.

### 6. Executar o Desktop administrativo

```bash
cd BetaFit.Desktop
dotnet run
```

Requer Windows. Confirme que `BetaFitApi:BaseUrl` em `BetaFit.Desktop/appsettings.json` aponta
para a URL da API.

### 7. Executar os testes

```bash
dotnet test
```

## Endpoints principais

```
GET    /api/products
GET    /api/products/featured
GET    /api/products/{id}
GET    /api/products/{id}/related
POST   /api/products
PUT    /api/products/{id}
PATCH  /api/products/{id}/activate
PATCH  /api/products/{id}/deactivate
PATCH  /api/products/{id}/featured
DELETE /api/products/{id}

GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories
PUT    /api/categories/{id}
PATCH  /api/categories/{id}/activate
PATCH  /api/categories/{id}/deactivate
DELETE /api/categories/{id}

GET    /api/dashboard/summary
```

Detalhes completos de request/response em [`DOCUMENTATION.md`](DOCUMENTATION.md).

## Segurança

O projeto não implementa autenticação/autorização (escopo institucional/demonstrativo), mas a
arquitetura já está preparada para receber JWT/Identity no futuro: os Controllers são finos, a
Application não depende de `HttpContext`, e o Desktop já trata a área administrativa como um
cliente autenticável (bastaria adicionar um `DelegatingHandler` no `HttpClient`).

## Licença

Projeto de portfólio, livre para estudo e referência.


## Hardening desta versão
- API versionada em `/api/v1`.
- Soft delete no domínio para evitar remoções destrutivas.
- Consultas de leitura usam `AsNoTracking` quando apropriado.
- Índices compostos para catálogo e destaque.
- Busca sem `ToLower()` sobre colunas, preservando melhor o uso de índices/collation.
- Dashboard preparado para métricas agregadas.
- Bootstrap do banco resiliente quando o pacote é entregue sem migrations.
- Testes e documentação devem ser executados com o .NET 8 SDK instalado.

### Variáveis de produção
Defina `ConnectionStrings__DefaultConnection` e `Admin__ApiKey` no ambiente de execução. Não use a chave de desenvolvimento em produção.
