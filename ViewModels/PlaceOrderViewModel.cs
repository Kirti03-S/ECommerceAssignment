using ECommerceWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.ViewModels
{
    public class PlaceOrderViewModel
    {
        [Required]
        public string Address { get; set; }

        public List<CartItem> CartItems { get; set; }
    }

}
