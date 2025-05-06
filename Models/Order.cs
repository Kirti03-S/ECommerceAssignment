using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace ECommerceWeb.Models
    {
  

    public enum OrderStatus
    {
        Placed,
        Shipping,
        Delivered
    }

    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Placed; // Default status
    }



}
