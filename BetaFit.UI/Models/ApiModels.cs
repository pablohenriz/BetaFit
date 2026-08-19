namespace BetaFit.UI.Models;

// Estes modelos espelham os contratos (DTOs) expostos pela BetaFit.API.
// O Website nunca acessa entidades de domínio ou o banco diretamente.

public enum Gender
{
    Unissex = 0,
    Masculino = 1,
    Feminino = 2
}

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}

public class ProductListItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
}

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
}

public class PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CatalogQuery
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Gender? Gender { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 1;
}
