using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;
using BetaFit.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BetaFit.Domain.Tests;

public class ProductTests
{
    private static Product CreateValidProduct() =>
        new("Camiseta Dry", "Descrição", 99.90m, "/img.jpg", Guid.NewGuid(), Gender.Masculino);

    [Fact]
    public void Should_Create_Product_With_Valid_Data()
    {
        var product = CreateValidProduct();

        product.IsActive.Should().BeTrue();
        product.IsFeatured.Should().BeFalse();
        product.Price.Should().Be(99.90m);
    }

    [Fact]
    public void Should_Throw_When_Price_Is_Negative()
    {
        var act = () => new Product("Camiseta", "Descrição", -10m, null, Guid.NewGuid(), Gender.Unissex);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_Throw_When_CategoryId_Is_Empty()
    {
        var act = () => new Product("Camiseta", "Descrição", 10m, null, Guid.Empty, Gender.Unissex);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_Mark_And_Unmark_As_Featured()
    {
        var product = CreateValidProduct();

        product.MarkAsFeatured();
        product.IsFeatured.Should().BeTrue();

        product.UnmarkAsFeatured();
        product.IsFeatured.Should().BeFalse();
    }

    [Fact]
    public void Should_Activate_And_Deactivate_Product()
    {
        var product = CreateValidProduct();

        product.Deactivate();
        product.IsActive.Should().BeFalse();

        product.Activate();
        product.IsActive.Should().BeTrue();
    }
}
