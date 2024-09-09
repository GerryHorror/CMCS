/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// Action method for the Index page
        public IActionResult Index()
        {
            // If user is not logged in, redirect to Login
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        /// Action method for the Submit page
        public IActionResult Submit()
        {
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        /// Action method for the Claim page
        public IActionResult Claim()
        {
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for the User page
        public IActionResult User()
        {
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for the Verify page
        public IActionResult Verify()
        {
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for the Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for the Manage page
        public IActionResult Manage()
        {
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for the Login page
        public IActionResult Login()
        {
            // If user is already logged in, redirect to Index
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for the Login page
        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            // Dummy login for prototype purposes only - this will be replaced with a proper login system in the future
            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetString("Username", username);
            return RedirectToAction("Index");
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for logging out
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // <-------------------------------------------------------------------------------------->

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}