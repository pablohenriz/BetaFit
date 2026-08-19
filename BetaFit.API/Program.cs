using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BetaFit.API.Extensions;
using BetaFit.API.Middleware;
using BetaFit.Application.Common;
using BetaFit.Infrastructure;
using BetaFit.Infrastructure.Context;
using BetaFit.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Dependency Injection: cada camada expõe sua própria extensão de DI,
// mantendo o Program.cs simples e a composição da aplicação explícita.
// ---------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBetaFitCors(builder.Configuration);
builder.Services.AddBetaFitSwagger();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key não configurada.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BetaFit";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// ---------------------------------------------------------------------
// Aplica migrations e popula o banco com dados fictícios na primeira execução.
// ---------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BetaFitDbContext>();
    await context.Database.EnsureCreatedAsync();
    await BetaFitDbSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Beta Fit API v1");
    });
}

app.UseBetaFitExceptionHandling();
app.UseHttpsRedirection();
app.UseResponseCompression();

app.UseCors(BetaFit.API.Extensions.CorsServiceCollectionExtensions.PolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Necessário para permitir que o WebApplicationFactory (testes de integração) enxergue a classe Program.
public partial class Program { }
