using BCrypt.Net;
using ECommerceWeb.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OrderInvoiceSystem.Models;
using System;
using System.Security.Claims;
using ECommerceWeb.ViewModels;

namespace ECommerceWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel customer)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingCustomer = _db.Customers.FirstOrDefault(c => c.Email == customer.Email);
                if (existingCustomer != null)
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(customer);
                }

                // Hash password 
                string hash = BCrypt.Net.BCrypt.HashPassword(customer.Password); // hash it

                // Save new customer to DB with Role
                _db.Customers.Add(new Customer
                {
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    Password = hash,
                    Role = "User" 
                });

              
                _db.SaveChanges();

                TempData["success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            return View(customer);
        }


        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel customer, string? returnUrl = null)
        {
            var user = _db.Customers.SingleOrDefault(u => u.Email == customer.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(customer.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            // Create the user's claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.CustomerName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create identity and principal
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in using cookie authentication
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect based on role
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (user.Role == "Admin")
            {
                return RedirectToAction("Manage", "Product"); // Admin landing page
            }
            else
            {
                return RedirectToAction("Index", "Product"); // User landing page
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

