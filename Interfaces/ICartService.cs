using ECommerceWeb.Models;
using System.Collections.Generic;
using ECommerceWeb.Services;

public interface ICartService
{
    List<CartItem> GetCart();
    void SaveCart(List<CartItem> cart);
    void AddToCart(Product product);
    void UpdateQuantity(int productId, int quantity);
    void RemoveFromCart(int productId);
    void ClearCart();
}

