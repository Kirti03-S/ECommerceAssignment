using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string CustomerName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Compare("Password")]
        [Required(ErrorMessage = "Confirm your password")]
        public string ConfirmPassword { get; set; }


    }
}
