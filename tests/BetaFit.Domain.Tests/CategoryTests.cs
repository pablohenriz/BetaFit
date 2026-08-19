using BetaFit.Domain.Entities;
using BetaFit.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BetaFit.Domain.Tests;

public class CategoryTests
{
    [Fact]
    public void Should_Create_Category_With_Valid_Data()
    {
        var category = new Category("Camisetas", "Camisetas leves e respiráveis.", "/img.jpg");

        category.Name.Should().Be("Camisetas");
        category.IsActive.Should().BeTrue();
        category.Products.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Should_Throw_When_Name_Is_Empty(string? name)
    {
        var act = () => new Category(name!, "Descrição válida", null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_Deactivate_And_Activate_Category()
    {
        var category = new Category("Shorts", "Descrição", null);

        category.Deactivate();
        category.IsActive.Should().BeFalse();

        category.Activate();
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Should_Update_Category_Fields()
    {
        var category = new Category("Shorts", "Descrição antiga", null);

        category.Update("Shorts Training", "Nova descrição", "/nova-imagem.jpg");

        category.Name.Should().Be("Shorts Training");
        category.Description.Should().Be("Nova descrição");
        category.ImageUrl.Should().Be("/nova-imagem.jpg");
        category.UpdatedAt.Should().NotBeNull();
    }
}
