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
        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }
    }
}
