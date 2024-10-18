/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

/* This controller is used to handle the approval of claims. It includes a method to retrieve claims for verification and a method to update the status of a claim. */

using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ApprovalController : Controller
    {
        // Declare the database context and logger

        private readonly CMCSDbContext _context;
        private readonly ILogger<ApprovalController> _logger;

        // Constructor to initialise the database context and logger
        public ApprovalController(CMCSDbContext context, ILogger<ApprovalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Method to retrieve claims for verification and return the view
        public async Task<IActionResult> Verify()
        {
            // Try to retrieve claims from the database and return the view
            try
            {
                // Retrieve claims from the database, include related entities, order by submission date and return the view
                var claims = await _context.Claims.Include(c => c.User).Include(c => c.Status).OrderByDescending(c => c.SubmissionDate).ToListAsync();

                return View(claims);
            }
            // Catch any exceptions and log the error
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving claims for verification");
                // Return a status code 500 with an error message if an exception occurs
                return StatusCode(500, "An error occurred while retrieving claims. Please try again later.");
            }
        }

        // <-------------------------------------------------------------------------------------------------------------------------------------------->

        // Method to update the status of a claim and return a success message
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusModel model)
        {
            // Check if the model state is valid (i.e. all required fields are present and have valid values)
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid model state", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            // Try to update the status of the claim and return a success message
            try
            {
                var claim = await _context.Claims.FindAsync(model.ClaimId);
                if (claim == null)
                {
                    return NotFound(new { success = false, message = $"Claim with ID {model.ClaimId} not found" });
                }

                var newStatus = await _context.ClaimStatuses.FirstOrDefaultAsync(s => s.StatusName == model.Status);
                if (newStatus == null)
                {
                    return BadRequest(new { success = false, message = $"Invalid status: {model.Status}" });
                }

                claim.StatusID = newStatus.StatusID;
                if (model.Status == "Approved" || model.Status == "Rejected")
                {
                    claim.ApprovalDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Claim {ClaimId} status updated to {Status}", model.ClaimId, model.Status);
                return Json(new { success = true, message = $"Claim {model.ClaimId} has been {model.Status.ToLower()}." });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating claim {ClaimId} status", model.ClaimId);
                return StatusCode(500, new { success = false, message = "A database error occurred while updating the claim status." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating claim {ClaimId} status", model.ClaimId);
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while updating the claim status." });
            }
        }

        // <-------------------------------------------------------------------------------------------------------------------------------------------->

        // Method to retrieve details of a claim and return the details as JSON
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Retrieve the claim details from the database, include related entities and return the details as JSON
                var claim = await _context.Claims.Include(c => c.User).Include(c => c.Status).FirstOrDefaultAsync(c => c.ClaimID == id);
                // Check if the claim exists
                if (claim == null)
                {
                    return NotFound(new { success = false, message = $"Claim with ID {id} not found" });
                }

                // Retrieve the document names associated with the claim
                var documents = await _context.Documents.Where(d => d.ClaimID == id).Select(d => d.DocumentName).ToListAsync();
                // Return the claim details and associated documents as JSON
                return Json(new
                {
                    success = true,
                    claim.ClaimID,
                    claim.User.FirstName,
                    claim.User.LastName,
                    claim.SubmissionDate,
                    claim.ClaimAmount,
                    claim.Status.StatusName,
                    claim.HoursWorked,
                    claim.HourlyRate,
                    claim.ClaimType,
                    Documents = documents
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving details for claim {ClaimId}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving claim details." });
            }
        }
    }
}