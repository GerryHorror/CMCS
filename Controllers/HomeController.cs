/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
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
        private readonly ILogger<HomeController> _logger; // Logger for logging information, warnings, and errors
        private readonly CMCSDbContext _context; // Database context for accessing the database

        // Constructor to initialise the logger and database context
        public HomeController(ILogger<HomeController> logger, CMCSDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Action to display the home page
        public IActionResult Index()
        {
            try
            {
                // Check if the user is authenticated by verifying the presence of a user role in the session
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
                {
                    _logger.LogInformation("Redirecting unauthenticated user to Login");
                    return RedirectToAction("Login"); // Redirect to login page if not authenticated
                }
                return View(); // Return the home page view
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred in the Index action");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to display the privacy policy page
        public IActionResult Privacy()
        {
            try
            {
                return View(); // Return the privacy policy view
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred in the Privacy action");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to display the login form (GET request)
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Return the login form view
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to handle the login form submission (POST request)
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt: ModelState is invalid");
                return View(model); // Return the login form view with validation errors
            }

            try
            {
                // Retrieve the user from the database based on the provided username
                var user = await _context.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName == model.Username);

                // Verify the user's password and check if the user exists
                if (user != null && VerifyPassword(model.Password, user.UserPassword))
                {
                    // Set session variables for the authenticated user
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.Role.RoleName);
                    _logger.LogInformation("User {Username} logged in successfully", user.UserName);
                    return RedirectToAction("Index", "Home"); // Redirect to the home page after successful login
                }

                // Log a warning if the login attempt fails
                _logger.LogWarning("Failed login attempt for username: {Username}", model.Username);
                ModelState.AddModelError("", "Invalid username or password");
                return View(model); // Return the login form view with an error message
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs during login
                _logger.LogError(ex, "An error occurred during login for username: {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model); // Return the login form view with an error message
            }
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to handle user logout
        public IActionResult Logout()
        {
            try
            {
                // Retrieve the username from the session
                var username = HttpContext.Session.GetString("Username");
                // Clear the session to log out the user
                HttpContext.Session.Clear();
                _logger.LogInformation("User {Username} logged out", username);
                return RedirectToAction("Index"); // Redirect to the home page after logout
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs during logout
                _logger.LogError(ex, "An error occurred during logout");
                return StatusCode(500, "An unexpected error occurred during logout. Please try again.");
            }
        }

        // <-------------------------------------------------------------------------------------------------->

        // Method to verify the user's password
        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Simple password verification. In a real-world application, this should be more secure (e.g., hashing)
            return inputPassword == storedPassword;
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to display the error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Return the error view with the request ID
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}