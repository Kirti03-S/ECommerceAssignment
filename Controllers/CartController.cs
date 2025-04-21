using Microsoft.AspNetCore.Mvc;
using ECommerceWeb.Models;
using ECommerceWeb.Helpers;
using ECommerceWeb.Data;

public class CartController : Controller
{
    private const string CartSessionKey = "Cart";
    private readonly ApplicationDbContext _db; // Replace 'object' with your actual DbContext type  

    public CartController(ApplicationDbContext db) // Inject the DbContext via constructor  
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        return View(cart);
    }

    public IActionResult AddToCart(int id)
    {
        var product = _db.Products.Find(id); // This will now work if 'Products' is a DbSet in your DbContext  
        if (product == null) return NotFound();

        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(c => c.ProductId == id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1
            });
        }

        HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        return RedirectToAction("Index", "Cart");
    }

    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
        var item = cart?.FirstOrDefault(c => c.ProductId == id);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        }
        return RedirectToAction("Index");
    }
}
