using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerceWeb.Models;
using ECommerceWeb.Data;
using ECommerceWeb.Helpers;
using OrderInvoiceSystem.Models;
using Microsoft.EntityFrameworkCore;
using OrderInvoiceSystem.Models;
using ECommerceWeb.ViewModels;
using System.Text.Json;

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



    [HttpPost]
    public async Task<IActionResult> PlaceOrder(string address)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

        // Check if cart is empty (basic validation)
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        // Only validate address if the form was submitted
        if (Request.Method == "POST" && string.IsNullOrWhiteSpace(address))
        {
            TempData["Error"] = "Please provide a valid shipping address.";
            TempData["Cart"] = JsonSerializer.Serialize(cart);
            return RedirectToAction("Checkout");
        }

        // Proceed with order placement if validation passes
        var order = new Order
        {
            Address = address,
            OrderDate = DateTime.Now,
            Items = cart.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Price
            }).ToList(),
            TotalAmount = cart.Sum(x => x.Price * x.Quantity),
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        HttpContext.Session.Remove("Cart");
        TempData["Success"] = "Order placed successfully!";
        return RedirectToAction("MyOrders");
    }

    public IActionResult Checkout()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

        // If cart is empty, redirect to cart page
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        // Restore cart from TempData if redirected back from PlaceOrder
        if (TempData["Cart"] != null)
        {
            cart = JsonSerializer.Deserialize<List<CartItem>>(TempData["Cart"].ToString());
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        return View(cart);
    }

}
