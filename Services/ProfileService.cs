using System.Security.Claims;
using ECommerceWeb.Models;
using ECommerceWeb.ViewModels;
using Microsoft.AspNetCore.Http;

public class ProfileService : IProfileService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProfileService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserProfileViewModel GetUserProfile(ClaimsPrincipal user)
    {
        return new UserProfileViewModel
        {
            UserName = user.Identity?.Name ?? "",
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "",
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? ""
        };
    }
}
