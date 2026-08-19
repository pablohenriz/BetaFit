using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using BetaFit.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly BetaFitDbContext _context;

    public CategoryRepository(BetaFitDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .AsQueryable();

        if (onlyActive)
            query = query.Where(c => c.IsActive);

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? ignoreId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.Where(c => c.Name == name.Trim());

        if (ignoreId.HasValue)
            query = query.Where(c => c.Id != ignoreId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => await _context.Products.AnyAsync(p => p.CategoryId == categoryId, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Categories.CountAsync(cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        => await _context.Categories.AddAsync(category, cancellationToken);

    public void Update(Category category)
        => _context.Categories.Update(category);

    public void Remove(Category category)
        => _context.Categories.Remove(category);
}
