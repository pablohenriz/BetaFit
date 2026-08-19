using System.Text.Json;
using BetaFit.UI.Models;
using Microsoft.AspNetCore.Http;

namespace BetaFit.UI.Services;

public static class CartService
{
    private const string Key = "betafit_cart";

    public static List<CartItem> Get(HttpContext context)
    {
        var json = context.Session.GetString(Key);
        return string.IsNullOrWhiteSpace(json)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
    }

    private static void Save(HttpContext context, List<CartItem> items)
        => context.Session.SetString(Key, JsonSerializer.Serialize(items));

    public static void Add(HttpContext context, ProductResponse product)
    {
        var items = Get(context);
        var item = items.FirstOrDefault(x => x.ProductId == product.Id);
        if (item is null)
            items.Add(new CartItem { ProductId = product.Id, Name = product.Name, Price = product.Price, ImageUrl = product.ImageUrl, Quantity = 1 });
        else item.Quantity++;
        Save(context, items);
    }

    public static void Remove(HttpContext context, Guid productId)
    {
        var items = Get(context);
        items.RemoveAll(x => x.ProductId == productId);
        Save(context, items);
    }

    public static decimal Total(HttpContext context) => Get(context).Sum(x => x.Price * x.Quantity);
}
