using ECommerceWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderService
{
    Task<string> PlaceOrderAsync(string userId, string address, List<CartItem> cart);
}

