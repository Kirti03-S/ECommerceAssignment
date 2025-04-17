using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        // carry over return URL so we can redirect after successful login
        public string? ReturnUrl { get; set; }
    }
}


