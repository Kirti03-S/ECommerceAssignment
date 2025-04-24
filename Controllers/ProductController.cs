using ECommerceWeb.Data;
using ECommerceWeb.Models;
using Microsoft.AspNetCore.Authorization;
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

        // GET: /Product/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /Product/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /Product/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

    }
}

