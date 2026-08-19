using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages;

public class CartModel : PageModel
{
    public List<CartItem> Items { get; private set; } = new();
    public decimal Total { get; private set; }

    public void OnGet(Guid? remove)
    {
        if (remove.HasValue) CartService.Remove(HttpContext, remove.Value);
        Items = CartService.Get(HttpContext);
        Total = CartService.Total(HttpContext);
    }
}
