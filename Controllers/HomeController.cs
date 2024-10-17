/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;
using static CMCS.Models.LoginModel;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CMCSDbContext _context;

        public HomeController(ILogger<HomeController> logger, CMCSDbContext context)
        {
            _logger = logger;
            _context = context;
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

        // Action method for the Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // <-------------------------------------------------------------------------------------->
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Action method for the Login page
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // Check if the user exists in the database
            if (ModelState.IsValid)
            {
                // Get the user from the database and check if the password is correct
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == model.Username);

                if (user != null && VerifyPassword(model.Password, user.UserPassword))
                {
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.Role.RoleName);
                    return RedirectToAction("Index", "Home");
                }
                // Message for invalid username or password
                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(model);
        }

        // <-------------------------------------------------------------------------------------->

        // Action method for logging out
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // <-------------------------------------------------------------------------------------->

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return inputPassword == storedPassword;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}