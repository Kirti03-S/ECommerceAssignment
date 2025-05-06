using ECommerceWeb.Models;
using System.Threading.Tasks;

public interface IProductService
{
    Task<Product> GetByIdAsync(int id);
    Task<bool> IsStockAvailableAsync(int productId, int desiredQuantity);
}

