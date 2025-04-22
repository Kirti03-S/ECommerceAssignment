using System.ComponentModel.DataAnnotations.Schema;

    namespace ECommerceWeb.Models
    {
        public class Order
        {
            public int Id { get; set; }
            public string UserId { get; set; } // Foreign key to Identity User
            public DateTime OrderDate { get; set; } = DateTime.Now;

            public List<OrderItem> Items { get; set; } = new();
        }

    }
