using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages.Admin;

[Authorize(Roles = "Admin")]
public class NewProductModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    private readonly IWebHostEnvironment _env;
    public NewProductModel(IBetaFitApiClient api, IWebHostEnvironment env) { _api = api; _env = env; }

    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public string Description { get; set; } = "";
    [BindProperty] public decimal Price { get; set; }
    [BindProperty] public Guid CategoryId { get; set; }
    [BindProperty] public Gender Gender { get; set; }
    [BindProperty] public bool IsFeatured { get; set; }
    [BindProperty] public IFormFile? Image { get; set; }
    public List<SelectListItem> CategoryOptions { get; private set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(CancellationToken ct) => await LoadCategories(ct);

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        await LoadCategories(ct);
        string? imageUrl = null;

        if (Image is not null && Image.Length > 0)
        {
            var ext = Path.GetExtension(Image.FileName).ToLowerInvariant();
            if (ext is not ".jpg" and not ".jpeg" and not ".png" and not ".webp")
            { ErrorMessage = "Use JPG, PNG ou WEBP."; return Page(); }

            var folder = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            await using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
            await Image.CopyToAsync(stream, ct);
            imageUrl = $"/images/products/{fileName}";
        }

        var created = await _api.CreateProductAsync(new CreateProductRequest
        {
            Name = Name, Description = Description, Price = Price,
            CategoryId = CategoryId, Gender = Gender, IsFeatured = IsFeatured, ImageUrl = imageUrl
        }, ct);

        if (created is null) { ErrorMessage = "Não foi possível salvar o produto."; return Page(); }
        return RedirectToPage("/Admin/Index");
    }

    private async Task LoadCategories(CancellationToken ct)
    {
        var categories = await _api.GetActiveCategoriesAsync(ct);
        CategoryOptions = categories.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
    }
}
