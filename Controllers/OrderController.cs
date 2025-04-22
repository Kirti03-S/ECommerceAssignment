using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerceWeb.Models;
using ECommerceWeb.Data;
using ECommerceWeb.Helpers;
using OrderInvoiceSystem.Models;
using Microsoft.EntityFrameworkCore;
using OrderInvoiceSystem.Models;

[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _db;

    public OrderController(ApplicationDbContext db)
    {
        _db = db;
    }
    public IActionResult MyOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var orders = _db.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ToList();

        return View(orders);
    }
    public IActionResult PlaceOrder()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
        if (cart == null || !cart.Any())
        {
            return RedirectToAction("Index", "Product");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = new Order
        {
            UserId = userId,
            Items = cart.Select(c => new OrderItem
            {
                ProductId = c.ProductId, // Fixed: Use ProductId directly from CartItem
                Quantity = c.Quantity,
                Price = c.Price 
            }).ToList()
        };

        _db.Orders.Add(order);
        _db.SaveChanges();

        HttpContext.Session.Remove("Cart");
        return RedirectToAction("MyOrders", "Order");

        TempData["success"] = "Order placed successfully!";

        
        return RedirectToAction("Index", "Home");
    }
}
