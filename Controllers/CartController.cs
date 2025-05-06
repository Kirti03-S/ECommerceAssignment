using ECommerceWeb.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;

    public CartController(ApplicationDbContext db, ICartService cartService, IOrderService orderService, IProductService productService)
    {
        _db = db;
        _cartService = cartService;
        _orderService = orderService;
        _productService = productService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View(cart);
    }

    public IActionResult AddToCart(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();

        _cartService.AddToCart(product);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            TempData["Error"] = "Product not found.";
            return RedirectToAction("Index");
        }

        if (quantity > product.CurrentStock)
        {
            TempData["Error"] = $"Only {product.CurrentStock} units of '{product.Name}' are available.";
            return RedirectToAction("Index");
        }

        _cartService.UpdateQuantity(productId, quantity);
        return RedirectToAction("Index");
    }


    public IActionResult Remove(int id)
    {
        _cartService.RemoveFromCart(id);
        return RedirectToAction("Index");
    }

    

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(string address)
    {
        var cart = _cartService.GetCart();

        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var errorMessage = await _orderService.PlaceOrderAsync(userId, address, cart);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        _cartService.ClearCart();
        return RedirectToAction("OrderConfirmation");
    }
}
