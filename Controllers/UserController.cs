/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

// This is the boiler plate code for the User Controller. This controller will be used to handle the users. Functionality will be added to this controller in the future.

using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class UserController : Controller
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(CMCSDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> User()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorised access attempt to User action");
                    return RedirectToAction("Login", "Home");
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId.Value);
                    return NotFound("User not found");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user details");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> User(UserModel userModel)
        {
            ModelState.Remove("UserName");
            ModelState.Remove("UserPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = HttpContext.Session.GetInt32("UserID");
                    if (!userId.HasValue)
                    {
                        _logger.LogWarning("Unauthorised update attempt");
                        return Json(new { success = false, message = "User not authenticated" });
                    }

                    var existingUser = await _context.Users.FindAsync(userId.Value);
                    if (existingUser == null)
                    {
                        _logger.LogWarning("User not found for update. ID: {UserId}", userId.Value);
                        return Json(new { success = false, message = "User not found" });
                    }

                    // Update only specific fields
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

        public async Task<IActionResult> Manage()
        {
            try
            {
                var viewModel = new ManageViewModel
                {
                    Users = await _context.Users.Include(u => u.Role).ToListAsync(),
                    Roles = await _context.Roles.ToListAsync()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user management data");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLecturer([FromForm] UserModel userModel)
        {
            _logger.LogInformation("Received AddLecturer request: {UserModel}", System.Text.Json.JsonSerializer.Serialize(userModel));

            userModel.Role = null;

            if (ModelState.IsValid)
            {
                try
                {
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

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("Invalid model state in AddLecturer. Errors: {Errors}", string.Join(", ", errors));
            return Json(new { success = false, message = "Invalid data", errors });
        }
    }
}