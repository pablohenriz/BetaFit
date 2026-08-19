using BetaFit.Application.DTOs.Product;
using BetaFit.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BetaFit.API.Controllers;

/// <summary>
/// Endpoints de Product, consumidos tanto pelo Website público (catálogo)
/// quanto pelo Desktop administrativo (CRUD completo).
/// </summary>
[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Pesquisa produtos com filtros, ordenação e paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductListItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] ProductQueryRequest query, CancellationToken cancellationToken)
    {
        var result = await _productService.SearchAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Retorna os produtos em destaque para a Home do site.</summary>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductListItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatured([FromQuery] int take = 8, CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetFeaturedAsync(take, cancellationToken);
        return Ok(products);
    }

    /// <summary>Obtém um produto pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    /// <summary>Retorna produtos relacionados (mesma categoria) para a página de detalhe.</summary>
    [HttpGet("{id:guid}/related")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductListItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRelated(Guid id, [FromQuery] int take = 4, CancellationToken cancellationToken = default)
    {
        var related = await _productService.GetRelatedAsync(id, take, cancellationToken);
        return Ok(related);
    }

    /// <summary>Cria um novo produto. Usado pelo Desktop administrativo.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Atualiza um produto existente.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productService.UpdateAsync(id, request, cancellationToken);
        return Ok(product);
    }

    /// <summary>Ativa um produto.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await _productService.ActivateAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Desativa um produto.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _productService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Marca ou desmarca um produto como destaque.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/featured")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetFeatured(Guid id, [FromQuery] bool isFeatured, CancellationToken cancellationToken)
    {
        await _productService.SetFeaturedAsync(id, isFeatured, cancellationToken);
        return NoContent();
    }

    /// <summary>Exclui um produto.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
