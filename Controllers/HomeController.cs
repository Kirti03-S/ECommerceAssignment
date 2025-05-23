using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerceWeb.Models;

namespace ECommerceWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Manage", "Product");
                }
                else
                {
                    return RedirectToAction("Index", "Product"); 
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View(); 
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
