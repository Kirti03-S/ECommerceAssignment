using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace ECommerceWeb.Models
    {
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }

        // Add the missing TotalAmount property
        public decimal TotalAmount { get; set; }
        public string Address { get; set; } = string.Empty;
    }

    }
