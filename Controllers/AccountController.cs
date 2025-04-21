using BCrypt.Net;
using ECommerceWeb.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OrderInvoiceSystem.Models;
using System;
using System.Security.Claims;
using ECommerceWeb.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ECommerceWeb.ViewModels;

namespace YourApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel customer)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Username & password required");
                return View();
            }

            // 1) Check if user exists
            if (_db.Customers.Any(u => u.Email == customer.Email))
            {
                ModelState.AddModelError("", "Email taken");
                return View();
            }

            // 2) Hash the password
            string hash = BCrypt.Net.BCrypt.HashPassword(customer.Password);

            // 3) Save user
            _db.Customers.Add(new Customer
            {
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                Password = hash
            });
            await _db.SaveChangesAsync();

            // 4) Redirect to Login
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel customer, string? returnUrl = null)
        {
            var user = _db.Customers.SingleOrDefault(u => u.Email == customer.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(customer.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            // 1) Create the user's claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.CustomerName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            // 2) Create identity and principal (this is the part you were missing!)
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity); // ← this must be declared before using

            // 3) Sign in using cookie auth
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // 4) Redirect to home or returnUrl
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

