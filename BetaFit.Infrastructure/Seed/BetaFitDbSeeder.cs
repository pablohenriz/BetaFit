using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;
using BetaFit.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.Infrastructure.Seed;

/// <summary>
/// Popula o banco com categorias e produtos fictícios para fins de demonstração/portfólio.
/// Executado uma única vez, na inicialização da API, se o banco estiver vazio.
/// </summary>
public static class BetaFitDbSeeder
{
    public static async Task SeedAsync(BetaFitDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!await context.Users.AnyAsync())
        {
            context.Users.Add(new User(
                "Administrador BetaFit",
                "admin@betafit.local",
                "6hiBSNbcYfVC6UlmLdcUKw==.siLrQMoO4J6MQbomVnEnKEqXnV5PX7GNmLa2kFroabY=",
                "Admin"));
            await context.SaveChangesAsync();
        }

        if (await context.Categories.AnyAsync())
            return;

        var masculino = new Category("Masculino", "Roupas fitness pensadas para o treino masculino.", "/images/categories/masculino.jpg");
        var feminino = new Category("Feminino", "Roupas fitness pensadas para o treino feminino.", "/images/categories/feminino.jpg");
        var camisetas = new Category("Camisetas", "Camisetas leves e respiráveis para qualquer treino.", "/images/categories/camisetas.jpg");
        var regatas = new Category("Regatas", "Regatas para dias intensos na academia.", "/images/categories/regatas.jpg");
        var shorts = new Category("Shorts", "Shorts de alta performance para treinos funcionais.", "/images/categories/shorts.jpg");
        var leggings = new Category("Leggings", "Leggings com compressão e sustentação para o treino.", "/images/categories/leggings.jpg");
        var calcas = new Category("Calças", "Calças de treino com tecido tecnológico.", "/images/categories/calcas.jpg");
        var moletons = new Category("Moletons", "Moletons para o pré e pós-treino.", "/images/categories/moletons.jpg");
        var acessorios = new Category("Acessórios", "Itens complementares para o seu treino.", "/images/categories/acessorios.jpg");

        await context.Categories.AddRangeAsync(
            masculino, feminino, camisetas, regatas, shorts, leggings, calcas, moletons, acessorios);

        var products = new List<Product>
        {
            new("Camiseta Dry Performance", "Camiseta com tecido de secagem rápida, ideal para treinos intensos.", 99.90m, "/images/products/camiseta-dry.jpg", camisetas.Id, Gender.Masculino),
            new("Camiseta Oversized Beta", "Camiseta oversized com modelagem moderna e conforto absoluto.", 109.90m, "/images/products/camiseta-oversized.jpg", camisetas.Id, Gender.Unissex),
            new("Regata Fit Muscle", "Regata com recorte estratégico para liberdade de movimento.", 89.90m, "/images/products/regata-muscle.jpg", regatas.Id, Gender.Masculino),
            new("Regata Feminina Cropped", "Regata cropped com tecido leve e respirável.", 84.90m, "/images/products/regata-cropped.jpg", regatas.Id, Gender.Feminino),
            new("Short Training Beta", "Short leve com bolsos laterais e cós elástico.", 119.90m, "/images/products/short-training.jpg", shorts.Id, Gender.Masculino),
            new("Short Feminino Performance", "Short com forro interno e tecido de alta compressão.", 124.90m, "/images/products/short-performance.jpg", shorts.Id, Gender.Feminino),
            new("Legging High Support", "Legging de cintura alta com sustentação total.", 169.90m, "/images/products/legging-high-support.jpg", leggings.Id, Gender.Feminino),
            new("Legging Seamless Beta", "Legging sem costuras, efeito segunda pele.", 179.90m, "/images/products/legging-seamless.jpg", leggings.Id, Gender.Feminino),
            new("Calça Jogger Training", "Calça jogger com punho e tecido tecnológico.", 189.90m, "/images/products/calca-jogger.jpg", calcas.Id, Gender.Masculino),
            new("Calça Flare Fitness", "Calça flare feminina para treino e uso casual.", 194.90m, "/images/products/calca-flare.jpg", calcas.Id, Gender.Feminino),
            new("Moletom Beta Fit Classic", "Moletom com capuz e bolso canguru, ideal para o pré-treino.", 219.90m, "/images/products/moletom-classic.jpg", moletons.Id, Gender.Unissex),
            new("Moletom Cropped Feminino", "Moletom cropped com acabamento premium.", 199.90m, "/images/products/moletom-cropped.jpg", moletons.Id, Gender.Feminino),
            new("Mochila Beta Training", "Mochila resistente com compartimento para tênis.", 249.90m, "/images/products/mochila-training.jpg", acessorios.Id, Gender.Unissex),
            new("Munhequeira Beta Fit", "Par de munhequeiras para apoio em treinos de força.", 39.90m, "/images/products/munhequeira.jpg", acessorios.Id, Gender.Unissex),
            new("Squeeze Beta 1L", "Squeeze térmico de 1 litro com trava de segurança.", 59.90m, "/images/products/squeeze.jpg", acessorios.Id, Gender.Unissex),
        };

        products[0].MarkAsFeatured();
        products[2].MarkAsFeatured();
        products[6].MarkAsFeatured();
        products[10].MarkAsFeatured();
        products[12].MarkAsFeatured();

        await context.Products.AddRangeAsync(products);

        await context.SaveChangesAsync();
    }
}
