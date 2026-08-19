using BetaFit.Application.DTOs.Product;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Exceptions;
using BetaFit.Domain.Interfaces;
using DomainProduct = BetaFit.Domain.Entities.Product;

namespace BetaFit.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) { _productRepository = productRepository; _categoryRepository = categoryRepository; _unitOfWork = unitOfWork; }
    public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken ct = default) { var p = await _productRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(DomainProduct), id); return Map(p); }
    public async Task<PagedResponse<ProductListItemResponse>> SearchAsync(ProductQueryRequest q, CancellationToken ct = default)
    { var result = await _productRepository.SearchAsync(new ProductQuery { SearchTerm=q.SearchTerm, CategoryId=q.CategoryId, Gender=q.Gender, IsActive=q.IsActive, IsFeatured=q.IsFeatured, SortBy=q.SortBy, Page=q.Page<=0?1:q.Page, PageSize=q.PageSize is <=0 or >100?12:q.PageSize }, ct); return new() { Items=result.Items.Select(MapList).ToList(), TotalCount=result.TotalCount, Page=result.Page, PageSize=result.PageSize, TotalPages=result.TotalPages }; }
    public async Task<IReadOnlyList<ProductListItemResponse>> GetFeaturedAsync(int take=8, CancellationToken ct=default) => (await _productRepository.GetFeaturedAsync(take,ct)).Select(MapList).ToList();
    public async Task<IReadOnlyList<ProductListItemResponse>> GetRelatedAsync(Guid id,int take=4,CancellationToken ct=default) { var p=await _productRepository.GetByIdAsync(id,ct)??throw new NotFoundException(nameof(DomainProduct),id); return (await _productRepository.GetRelatedAsync(id,p.CategoryId,take,ct)).Select(MapList).ToList(); }
    public async Task<ProductResponse> CreateAsync(CreateProductRequest r,CancellationToken ct=default) { var c=await _categoryRepository.GetByIdAsync(r.CategoryId,ct)??throw new DomainException("A categoria informada não existe."); var p=new DomainProduct(r.Name,r.Description,r.Price,r.ImageUrl,r.CategoryId,r.Gender); if(r.IsFeatured)p.MarkAsFeatured(); await _productRepository.AddAsync(p,ct); await _unitOfWork.SaveChangesAsync(ct); return Map(p,c.Name); }
    public async Task<ProductResponse> UpdateAsync(Guid id,UpdateProductRequest r,CancellationToken ct=default) { var p=await _productRepository.GetByIdAsync(id,ct)??throw new NotFoundException(nameof(DomainProduct),id); var c=await _categoryRepository.GetByIdAsync(r.CategoryId,ct)??throw new DomainException("A categoria informada não existe."); p.Update(r.Name,r.Description,r.Price,r.ImageUrl,r.CategoryId,r.Gender); _productRepository.Update(p); await _unitOfWork.SaveChangesAsync(ct); return Map(p,c.Name); }
    public async Task ActivateAsync(Guid id,CancellationToken ct=default)=>await ChangeActive(id,true,ct);
    public async Task DeactivateAsync(Guid id,CancellationToken ct=default)=>await ChangeActive(id,false,ct);
    private async Task ChangeActive(Guid id,bool active,CancellationToken ct){var p=await _productRepository.GetByIdAsync(id,ct)??throw new NotFoundException(nameof(DomainProduct),id);if(active)p.Activate();else p.Deactivate();_productRepository.Update(p);await _unitOfWork.SaveChangesAsync(ct);}
    public async Task SetFeaturedAsync(Guid id,bool featured,CancellationToken ct=default){var p=await _productRepository.GetByIdAsync(id,ct)??throw new NotFoundException(nameof(DomainProduct),id);if(featured)p.MarkAsFeatured();else p.UnmarkAsFeatured();_productRepository.Update(p);await _unitOfWork.SaveChangesAsync(ct);}
    public async Task DeleteAsync(Guid id,CancellationToken ct=default){var p=await _productRepository.GetByIdAsync(id,ct)??throw new NotFoundException(nameof(DomainProduct),id);p.Deactivate();p.MarkDeleted();_productRepository.Update(p);await _unitOfWork.SaveChangesAsync(ct);}
    private static ProductResponse Map(DomainProduct p,string? category=null)=>new(){Id=p.Id,Name=p.Name,Description=p.Description,Price=p.Price,ImageUrl=p.ImageUrl,CategoryId=p.CategoryId,CategoryName=category??p.Category?.Name??string.Empty,Gender=p.Gender,IsFeatured=p.IsFeatured,IsActive=p.IsActive,CreatedAt=p.CreatedAt,UpdatedAt=p.UpdatedAt};
    private static ProductListItemResponse MapList(DomainProduct p)=>new(){Id=p.Id,Name=p.Name,Price=p.Price,ImageUrl=p.ImageUrl,CategoryName=p.Category?.Name??string.Empty,Gender=p.Gender,IsFeatured=p.IsFeatured,IsActive=p.IsActive};
}
