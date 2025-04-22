using System.ComponentModel.DataAnnotations.Schema;
namespace OrderInvoiceSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public ICollection<OrderItem> Items { get; set; }
    }

}