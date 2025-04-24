using Microsoft.AspNetCore.Mvc;
using ECommerceWeb.Models;
using ECommerceWeb.Data;
using ECommerceWeb.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

public class CartController : Controller
{
    private const string CartSessionKey = "Cart";
    private readonly ApplicationDbContext _db;

    public CartController(ApplicationDbContext db)
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
        var product = _db.Products.FirstOrDefault(p => p.Id == id);
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
        return RedirectToAction("Index","Cart");
    }

    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        var item = cart?.FirstOrDefault(c => c.ProductId == id);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        }
        return RedirectToAction("Index");
    }


    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
        }

        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return RedirectToAction("Index");
    }

    public IActionResult PlaceOrder()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        return View(cart);
    }
    [HttpPost]
    public IActionResult PlaceOrder(string Address)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        var orderItems = cart.Select(item => new OrderItem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Price
        }).ToList();

        var order = new Order
        {
            Address = Address,
            OrderDate = DateTime.Now,
            Items = orderItems,
            TotalAmount = cart.Sum(x => x.Price * x.Quantity),
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        };

        // TODO: Save the order to database (e.g., _context.Orders.Add(order))

        HttpContext.Session.Remove("Cart");

        return RedirectToAction("OrderConfirmation");
    }

}
