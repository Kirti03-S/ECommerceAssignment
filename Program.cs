using OrderInvoiceSystem.Models;
using ECommerceWeb.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ECommerceWeb.Models;


var builder = WebApplication.CreateBuilder(args);

// 1) MVC services
builder.Services.AddControllersWithViews();

// 2) EF Core
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3) Authentication & Authorization (single call)
builder.Services.AddAuthentication(options =>
{
    // Use the built-in cookie scheme for authenticate, challenge and sign-in
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // "Cookies"
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie.Name = "YourAppAuth";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

builder.Services.AddSession();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated(); // Ensure DB exists

    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Name = "T-Shirt", Description = "Cotton T-Shirt", Price = 499, ImageUrl = "/images/tshirt.jpg" },
            new Product { Name = "Sneakers", Description = "Running Shoes", Price = 1999, ImageUrl = "/images/shoes.jpg" }
        );
        context.SaveChanges();
    }
}


// 4) HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseSession(); // Add this after app.UseRouting()


app.UseRouting();

// **VERY IMPORTANT**: authentication must come before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
