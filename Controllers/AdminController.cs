using ECommerceWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) 
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateStock(int productId, int currentStock, int stockLimit)
        {
            var product = _db.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.CurrentStock = currentStock;
            
            _db.SaveChanges();

            TempData["Success"] = "Stock updated successfully!";
            return RedirectToAction("Manage");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }
    }
}
