using ECommerceWeb.Models;
using ECommerceWeb.Helpers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceWeb.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> GetCart()
        {
            var context = _httpContextAccessor.HttpContext;
            return context.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        public void SaveCart(List<CartItem> cart)
        {
            var context = _httpContextAccessor.HttpContext;
            context.Session.SetObjectAsJson(CartSessionKey, cart);
        }

        public void AddToCart(Product product)
        {
            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            var context = _httpContextAccessor.HttpContext;
            context.Session.Remove(CartSessionKey);
        }
    }
}
