using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BetaFit.UI.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditProductModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public EditProductModel(IBetaFitApiClient api) => _api = api;
    [BindProperty] public Guid Id { get; set; }
    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public string Description { get; set; } = "";
    [BindProperty] public decimal Price { get; set; }
    [BindProperty] public Guid CategoryId { get; set; }
    [BindProperty] public Gender Gender { get; set; }
    [BindProperty] public string? ImageUrl { get; set; }
    public List<SelectListItem> Categories { get; private set; } = new();
    public string? Message { get; set; }
    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        var p = await _api.GetProductByIdAsync(id, ct); if (p is null) return NotFound();
        Id=p.Id; Name=p.Name; Description=p.Description; Price=p.Price; CategoryId=p.CategoryId; Gender=p.Gender; ImageUrl=p.ImageUrl;
        await LoadCategories(ct); return Page();
    }
    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        await LoadCategories(ct);
        var ok = await _api.UpdateProductAsync(Id, new UpdateProductRequest { Name=Name, Description=Description, Price=Price, CategoryId=CategoryId, Gender=Gender, ImageUrl=ImageUrl }, ct);
        if (!ok) { Message="Não foi possível atualizar o produto."; return Page(); }
        return RedirectToPage("/Admin/Index");
    }
    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken ct)
    {
        await _api.DeleteProductAsync(id, ct); return RedirectToPage("/Admin/Index");
    }
    private async Task LoadCategories(CancellationToken ct) => Categories=(await _api.GetActiveCategoriesAsync(ct)).Select(x=>new SelectListItem(x.Name,x.Id.ToString())).ToList();
}
