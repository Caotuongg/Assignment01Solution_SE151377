using eStoreClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace eStoreClient.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AdminIndex()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            if (string.IsNullOrEmpty(role))
            {
                TempData["LoginFail"] = "You are not login";
                return RedirectToAction("Login", "Members");
            }
            if (role != "Admin")
            {
                TempData["LoginFail"] = "You do not have permission to access this function";
                return RedirectToAction("Login", "Members");
            }
            return View();
        }
        public IActionResult CustomerIndex()
        {
            var role = HttpContext.Session.GetString("Role");
            if (!string.IsNullOrEmpty(role))
            {
                ViewData["Role"] = role;
            }
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}