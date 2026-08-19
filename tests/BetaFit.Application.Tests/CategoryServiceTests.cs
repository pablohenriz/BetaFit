using BetaFit.Application.DTOs.Category;
using BetaFit.Application.Services;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Exceptions;
using BetaFit.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BetaFit.Application.Tests;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Category_When_Name_Is_Unique()
    {
        _categoryRepository.ExistsByNameAsync("Leggings", Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.CreateAsync(new CreateCategoryRequest
        {
            Name = "Leggings",
            Description = "Descrição",
            ImageUrl = null
        });

        result.Name.Should().Be("Leggings");
        await _categoryRepository.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Name_Already_Exists()
    {
        _categoryRepository.ExistsByNameAsync("Leggings", Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _sut.CreateAsync(new CreateCategoryRequest { Name = "Leggings", Description = "x" });

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_When_Category_Has_Products()
    {
        var category = new Category("Shorts", "Descrição", null);

        _categoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        _categoryRepository.HasProductsAsync(category.Id, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _sut.DeleteAsync(category.Id);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_NotFound_When_Category_Does_Not_Exist()
    {
        _categoryRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Category?)null);

        var act = () => _sut.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
