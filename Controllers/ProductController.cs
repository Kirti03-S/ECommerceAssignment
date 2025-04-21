using ECommerceWeb.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommerceWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Product/
        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }
    }
}

