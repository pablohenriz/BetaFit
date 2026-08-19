using BetaFit.Application.DTOs.Category;
using BetaFit.Application.DTOs.Product;
using BetaFit.Application.Validators;
using BetaFit.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BetaFit.API.Tests;

/// <summary>
/// Testa os validators usados pelos Controllers antes de qualquer chamada à Application/Domain,
/// garantindo que a API rejeite payloads inválidos com 400 (ver ExceptionHandlingMiddleware).
/// </summary>
public class RequestValidationTests
{
    [Fact]
    public void CreateCategoryRequest_Should_Be_Invalid_When_Name_Is_Empty()
    {
        var validator = new CreateCategoryRequestValidator();

        var result = validator.Validate(new CreateCategoryRequest { Name = "", Description = "x" });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateCategoryRequest_Should_Be_Valid_With_Correct_Data()
    {
        var validator = new CreateCategoryRequestValidator();

        var result = validator.Validate(new CreateCategoryRequest { Name = "Moletons", Description = "Descrição" });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateProductRequest_Should_Be_Invalid_When_Price_Is_Negative()
    {
        var validator = new CreateProductRequestValidator();

        var result = validator.Validate(new CreateProductRequest
        {
            Name = "Camiseta",
            Description = "Descrição",
            Price = -1,
            CategoryId = Guid.NewGuid(),
            Gender = Gender.Unissex
        });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateProductRequest_Should_Be_Invalid_When_CategoryId_Is_Empty()
    {
        var validator = new CreateProductRequestValidator();

        var result = validator.Validate(new CreateProductRequest
        {
            Name = "Camiseta",
            Description = "Descrição",
            Price = 50,
            CategoryId = Guid.Empty,
            Gender = Gender.Unissex
        });

        result.IsValid.Should().BeFalse();
    }
}
