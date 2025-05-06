using ECommerceWeb.Data;
using ECommerceWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerceWeb.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;

        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> IsStockAvailableAsync(int productId, int desiredQuantity)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return false;

            return desiredQuantity <= product.CurrentStock;
        }
    }
}