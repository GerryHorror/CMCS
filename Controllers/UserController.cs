/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

// This controller is used to handle the users. It contains actions to display user details and manage users. The User action retrieves the user details from the database and displays them. The User action also allows the user to update their details. The Manage action retrieves all users from the database and displays them. The AddLecturer action adds a new lecturer to the database.

using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class UserController : Controller
    {
        private readonly CMCSDbContext _context; // Database context for accessing the database
        private readonly ILogger<UserController> _logger; // Logger for logging information, warnings, and errors

        // Constructor to initialise the database context and logger
        public UserController(CMCSDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to display user details
        public async Task<IActionResult> User()
        {
            try
            {
                // Retrieve the user ID from the session
                var userId = HttpContext.Session.GetInt32("UserID");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorized access attempt to User action");
                    return RedirectToAction("Login", "Home"); // Redirect to login if user is not authenticated
                }

                // Retrieve the user from the database
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId.Value);
                    return NotFound("User not found"); // Return 404 if user is not found
                }

                return View(user); // Return the user details view
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user details");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to update user details (POST request)
        [HttpPost]
        public async Task<IActionResult> User(UserModel userModel)
        {
            // Remove validation for UserName and UserPassword as they are not being updated
            ModelState.Remove("UserName");
            ModelState.Remove("UserPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the user ID from the session
                    var userId = HttpContext.Session.GetInt32("UserID");
                    if (!userId.HasValue)
                    {
                        _logger.LogWarning("Unauthorized update attempt");
                        return Json(new { success = false, message = "User not authenticated" });
                    }

                    // Retrieve the existing user from the database
                    var existingUser = await _context.Users.FindAsync(userId.Value);
                    if (existingUser == null)
                    {
                        _logger.LogWarning("User not found for update. ID: {UserId}", userId.Value);
                        return Json(new { success = false, message = "User not found" });
                    }

                    // Update specific fields
                    existingUser.FirstName = userModel.FirstName;
                    existingUser.LastName = userModel.LastName;
                    existingUser.UserEmail = userModel.UserEmail;
                    existingUser.PhoneNumber = userModel.PhoneNumber;
                    existingUser.BankName = userModel.BankName;
                    existingUser.BranchCode = userModel.BranchCode;
                    existingUser.BankAccountNumber = userModel.BankAccountNumber;
                    existingUser.Address = userModel.Address;

                    // Update password if a new one is provided
                    if (!string.IsNullOrEmpty(userModel.UserPassword))
                    {
                        existingUser.UserPassword = userModel.UserPassword;
                    }

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User profile updated successfully. ID: {UserId}", userId.Value);

                    return Json(new { success = true, message = "Profile updated successfully" });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error occurred while updating user profile");
                    return Json(new { success = false, message = "A database error occurred while updating the profile", error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while updating user profile");
                    return Json(new { success = false, message = "An unexpected error occurred while updating the profile", error = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Invalid model state in User update. Errors: {Errors}", string.Join(", ", errors));
            return Json(new { success = false, message = "Invalid data", errors = errors });
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to display the user management page
        public async Task<IActionResult> Manage()
        {
            try
            {
                var viewModel = new ManageViewModel
                {
                    Users = await _context.Users.Include(u => u.Role).ToListAsync(),
                    Roles = await _context.Roles.ToListAsync()
                };
                return View(viewModel); // Return the user management view
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user management data");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // <-------------------------------------------------------------------------------------------------->

        // Action to add a new lecturer (POST request)
        [HttpPost]
        public async Task<IActionResult> AddLecturer([FromForm] UserModel userModel)
        {
            _logger.LogInformation("Received AddLecturer request: {UserModel}", System.Text.Json.JsonSerializer.Serialize(userModel));

            userModel.Role = null; // Ensure the role is not set directly

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for duplicate user
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                        u.UserName == userModel.UserName ||
                        u.UserEmail == userModel.UserEmail ||
                        u.PhoneNumber == userModel.PhoneNumber ||
                        (u.FirstName == userModel.FirstName && u.LastName == userModel.LastName));

                    if (existingUser != null)
                    {
                        string duplicateField = "";
                        if (existingUser.UserName == userModel.UserName) duplicateField = "username";
                        else if (existingUser.UserEmail == userModel.UserEmail) duplicateField = "email";
                        else if (existingUser.PhoneNumber == userModel.PhoneNumber) duplicateField = "phone number";
                        else if (existingUser.FirstName == userModel.FirstName && existingUser.LastName == userModel.LastName) duplicateField = "name";

                        return Json(new { success = false, message = $"A user with this {duplicateField} already exists." });
                    }

                    _context.Users.Add(userModel);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("New lecturer added successfully. ID: {UserId}", userModel.UserID);
                    return Json(new { success = true, id = userModel.UserID });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error occurred while adding new lecturer");
                    return Json(new { success = false, message = "Error saving to database", error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occurred while adding new lecturer");
                    return Json(new { success = false, message = "An unexpected error occurred", error = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            _logger.LogWarning("Invalid model state in AddLecturer. Errors: {Errors}", string.Join(", ", errors));
            return Json(new { success = false, message = "Invalid data", errors });
        }

        // <-------------------------------------------------------------------------------------------------->

        // Helper method to check for duplicate users
        private async Task<bool> IsUserDuplicate(UserModel userModel)
        {
            return await _context.Users.AnyAsync(u =>
                u.UserName == userModel.UserName ||
                u.UserEmail == userModel.UserEmail ||
                u.PhoneNumber == userModel.PhoneNumber ||
                (u.FirstName == userModel.FirstName && u.LastName == userModel.LastName));
        }
    }
}