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

        public async Task<IActionResult> Manage()
        {
            var roles = await _context.Roles.ToListAsync();
            var lecturers = await _context.Users.Include(u => u.Role).ToListAsync();

            var viewModel = new ManageViewModel
            {
                Roles = roles,
                Users = lecturers
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