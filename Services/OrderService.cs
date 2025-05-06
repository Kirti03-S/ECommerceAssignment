using ECommerceWeb.Data;
using ECommerceWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceWeb.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> PlaceOrderAsync(string userId, string address, List<CartItem> cart)
        {
            if (cart == null || !cart.Any())
            {
                return "Cart is empty.";
            }

            // Load all products in one query
            var productIds = cart.Select(c => c.ProductId).ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Check availability and reduce stock
            foreach (var item in cart)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    return $"Product with ID {item.ProductId} not found.";
                }

                if (product.CurrentStock <= 0)
                {
                    return $"'{product.Name}' is out of stock.";
                }

                if (item.Quantity > product.CurrentStock)
                {
                    return $"Only {product.CurrentStock} units of '{product.Name}' are available.";
                }

                // Decrease stock
                product.CurrentStock -= item.Quantity;

                // EF will track this change since `product` was loaded from _db.Products
            }




            // Create order
            var order = new Order
            {
                UserId = userId,
                Address = address,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Price * c.Quantity),
                Items = cart.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };
           
            _db.Orders.Add(order);

            // Save changes including stock reduction and new order
            await _db.SaveChangesAsync();

            return null;
        }


    }
}