using System.Security.Claims;
using ECommerceWeb.Models;
using ECommerceWeb.ViewModels;

public interface IProfileService
{
    UserProfileViewModel GetUserProfile(ClaimsPrincipal user);
}
