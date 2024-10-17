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

        public UserController(CMCSDbContext context)
        {
            _context = context;
        }

        // Display the User page
        public async Task<IActionResult> User()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.Users.FindAsync(userId.Value);

            return View(user);
        }

        // Edit the user details
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
                        return Json(new { success = false, message = "User not authenticated" });
                    }

                    var existingUser = await _context.Users.FindAsync(userId.Value);
                    if (existingUser == null)
                    {
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

                    return Json(new { success = true, message = "Profile updated successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while updating the profile", error = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Invalid data", errors = errors });
        }

        public async Task<IActionResult> Manage()
        {
            var viewModel = new ManageViewModel
            {
                Users = await _context.Users.Include(u => u.Role).ToListAsync(),
                Roles = await _context.Roles.ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddLecturer([FromForm] UserModel userModel)
        {
            // Log received data
            Console.WriteLine($"Received UserModel: {System.Text.Json.JsonSerializer.Serialize(userModel)}");

            // Explicitly set Role to null as we're not creating a new role here
            userModel.Role = null;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Users.Add(userModel);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, id = userModel.UserID });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return Json(new { success = false, message = "Error saving to database", error = ex.Message });
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            Console.WriteLine($"ModelState Errors: {string.Join(", ", errors)}");

            return Json(new { success = false, message = "Invalid data", errors });
        }
    }
}