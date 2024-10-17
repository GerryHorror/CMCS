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

        public IActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
                {
                    _logger.LogInformation("Redirecting unauthenticated user to Login");
                    return RedirectToAction("Login");
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Index action");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        public IActionResult Privacy()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Privacy action");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt: ModelState is invalid");
                return View(model);
            }

            try
            {
                var user = await _context.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName == model.Username);

                if (user != null && VerifyPassword(model.Password, user.UserPassword))
                {
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.Role.RoleName);
                    _logger.LogInformation("User {Username} logged in successfully", user.UserName);
                    return RedirectToAction("Index", "Home");
                }

                _logger.LogWarning("Failed login attempt for username: {Username}", model.Username);
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username: {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            try
            {
                var username = HttpContext.Session.GetString("Username");
                HttpContext.Session.Clear();
                _logger.LogInformation("User {Username} logged out", username);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout");
                return StatusCode(500, "An unexpected error occurred during logout. Please try again.");
            }
        }

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