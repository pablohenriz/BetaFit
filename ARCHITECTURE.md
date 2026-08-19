# Arquitetura — Beta Fit

Este documento descreve a responsabilidade de cada camada, o fluxo de comunicação entre elas e os
principais padrões aplicados no projeto.

## Visão geral

```
        ┌────────────────────┐        ┌────────────────────────┐
        │  BetaFit.UI         │        │  BetaFit.Desktop        │
        │  (Website público)  │        │  (Admin, WPF)            │
        └──────────┬──────────┘        └───────────┬─────────────┘
                   │  HTTP/JSON                    │  HTTP/JSON
                   └───────────────┬────────────────┘
                                   ▼
                          ┌─────────────────┐
                          │   BetaFit.API    │  Controllers finos, Middleware, Swagger, CORS
                          └────────┬─────────┘
                                   ▼
                          ┌─────────────────────┐
                          │ BetaFit.Application   │  Services (casos de uso), DTOs, Validators
                          └────────┬─────────────┘
                                   ▼
                          ┌─────────────────┐
                          │  BetaFit.Domain   │  Entities, regras de negócio, interfaces
                          └────────▲─────────┘
                                   │ implementa as interfaces do Domain
                          ┌────────┴─────────────┐
                          │ BetaFit.Infrastructure │  DbContext, Repositories, EF Core
                          └────────┬─────────────┘
                                   ▼
                            SQL Server (BetaFitDb)
```

A seta entre `Infrastructure` e `Domain` está invertida propositalmente: a Infrastructure
**depende** do Domain (implementa `ICategoryRepository`, `IProductRepository`, `IUnitOfWork`), e
não o contrário. Isso é o núcleo da Regra de Dependência da Clean Architecture: as camadas
internas (Domain) nunca conhecem as externas (Infrastructure, API, UI, Desktop).

## Responsabilidade de cada camada

### BetaFit.Domain

- Contém `Category` e `Product` (entidades ricas, com construtores e métodos que validam e
  aplicam as regras de negócio — nunca setters públicos soltos).
- Contém `Gender` (enum), `DomainException` e `NotFoundException`.
- Define as abstrações `ICategoryRepository`, `IProductRepository` e `IUnitOfWork`, que a
  Infrastructure implementa.
- **Não referencia** ASP.NET Core, EF Core, SQL Server, HTTP, UI ou Desktop — o `.csproj` não tem
  nenhum pacote NuGet.

### BetaFit.Application

- Implementa os casos de uso (`CategoryService`, `ProductService`, `DashboardService`) que
  orquestram os repositórios do Domain.
- Define os DTOs de entrada e saída (`CreateProductRequest`, `ProductResponse` etc.) — a API nunca
  expõe entidades de domínio diretamente.
- Usa **FluentValidation** para validar os requests antes de qualquer regra de negócio ser
  executada.
- Depende apenas do `BetaFit.Domain`.

### BetaFit.Infrastructure

- Implementa o acesso a dados com **Entity Framework Core**: `BetaFitDbContext`,
  `CategoryConfiguration`, `ProductConfiguration` (Fluent API), `CategoryRepository` e
  `ProductRepository`.
- Contém o `BetaFitDbSeeder`, que popula categorias e produtos fictícios na primeira execução.
- É a **única** camada do projeto com uma connection string ou referência a SQL Server.

### BetaFit.API

- Expõe os casos de uso via REST: `CategoriesController`, `ProductsController`,
  `DashboardController`.
- Os Controllers são finos: apenas validam o request (via `IValidator<T>`), chamam o serviço da
  Application e retornam o `IActionResult` adequado. Nenhuma regra de negócio vive aqui.
- `ExceptionHandlingMiddleware` centraliza a tradução de exceções em respostas HTTP:
  - `ValidationException` (FluentValidation) → 400
  - `DomainException` (Domain) → 400
  - `NotFoundException` (Domain) → 404
  - Qualquer outra exceção → 500 (logada, mensagem genérica ao cliente)
- CORS liberado apenas para as origens do Website; Swagger disponível em ambiente de
  desenvolvimento.

### BetaFit.UI (Website público)

- ASP.NET Core Razor Pages. Consome a API através de `IBetaFitApiClient` (HttpClient tipado).
- Não referencia `BetaFit.Domain`, `BetaFit.Application` nem `BetaFit.Infrastructure` — o
  `.csproj` não tem `ProjectReference` para nenhuma dessas camadas.
- Páginas: `Index` (Home), `Catalog` (catálogo com filtros/paginação) e `Product/Index` (detalhe
  do produto, com CTA "Tenho interesse" via WhatsApp — sem checkout real).

### BetaFit.Desktop (Painel administrativo)

- WPF (.NET 8) com padrão **MVVM**, usando `CommunityToolkit.Mvvm` (`ObservableProperty`,
  `RelayCommand`).
- `IBetaFitApiService` é o único canal de comunicação com o backend — implementado com
  `HttpClient` tipado, registrado via `Microsoft.Extensions.Hosting`/DI no `App.xaml.cs`.
- Não possui `DbContext`, connection string ou qualquer referência a EF Core/SQL Server.
- Views: `DashboardView`, `ProductsView`, `CategoriesView`, organizadas como um shell único
  (`MainWindow` + sidebar) que alterna a seção visível via `MainViewModel.CurrentSection`.

## Fluxo: Website → API → Application → Infrastructure → Database

1. O usuário abre `/Catalog` no Website.
2. `CatalogModel.OnGetAsync` chama `IBetaFitApiClient.SearchProductsAsync`.
3. O `HttpClient` faz `GET /api/products?...` na `BetaFit.API`.
4. `ProductsController.Search` delega para `IProductService.SearchAsync`.
5. `ProductService` monta um `ProductQuery` (Domain) e chama `IProductRepository.SearchAsync`.
6. `ProductRepository` (Infrastructure) consulta o `BetaFitDbContext` via EF Core/SQL Server.
7. O resultado (`Product` do Domain) sobe a pilha sendo mapeado para `ProductResponse` (DTO) na
   Application, e o Website renderiza a página com esses dados.

## Fluxo: Desktop → API → Application → Infrastructure → Database

1. O administrador clica em "Novo produto" e depois "Salvar" em `ProductsView`.
2. `ProductListViewModel.SaveCommand` chama `IBetaFitApiService.CreateProductAsync`.
3. O `HttpClient` faz `POST /api/products` na `BetaFit.API`.
4. `ProductsController.Create` valida o request com `IValidator<CreateProductRequest>` e delega
   para `IProductService.CreateAsync`.
5. `ProductService` verifica a categoria, instancia um `Product` (Domain, que valida suas próprias
   regras), chama `IProductRepository.AddAsync` e `IUnitOfWork.SaveChangesAsync`.
6. `ProductRepository`/`BetaFitDbContext` (Infrastructure) persistem no SQL Server.
7. A resposta (`ProductResponse`) volta para o Desktop, que atualiza a `DataGrid` e exibe a
   mensagem de sucesso.

## Entidades e relacionamento

```
Category (1) ────< (N) Product
```

- `Category`: `Id`, `Name`, `Description`, `ImageUrl`, `IsActive`, `CreatedAt`, `UpdatedAt`.
- `Product`: `Id`, `Name`, `Description`, `Price`, `ImageUrl`, `CategoryId`, `Gender`,
  `IsFeatured`, `IsActive`, `CreatedAt`, `UpdatedAt`.
- Um `Product` sempre pertence a uma `Category`. Uma `Category` só pode ser excluída se não tiver
  produtos vinculados (regra aplicada em `CategoryService.DeleteAsync`).

## Dependency Injection

Cada camada expõe sua própria extensão de `IServiceCollection`, mantendo o `Program.cs` da API
simples e a composição explícita:

- `BetaFit.Application` → `AddApplication()` (registra os Services e os Validators)
- `BetaFit.Infrastructure` → `AddInfrastructure(configuration)` (registra `DbContext`,
  repositórios e `IUnitOfWork`)
- `BetaFit.API` → `AddBetaFitCors(configuration)`, `AddBetaFitSwagger()`

O Desktop usa `Microsoft.Extensions.Hosting` para montar seu próprio container de DI
(`App.xaml.cs`), registrando apenas o `HttpClient` tipado e as ViewModels.

## Repository Pattern e DTOs

- **Repository Pattern**: o Domain define os contratos (`ICategoryRepository`,
  `IProductRepository`), a Infrastructure os implementa com EF Core. A Application nunca usa
  `DbContext` diretamente.
- **DTO Pattern**: a API nunca retorna entidades de domínio. Toda entrada/saída HTTP passa por
  `Create...Request`, `Update...Request` e `...Response`, mapeados manualmente nos Services da
  Application (sem AutoMapper, para manter o mapeamento explícito e simples).
