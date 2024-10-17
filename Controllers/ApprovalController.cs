/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

/* This is the boiler plate code for the Approval Controller. This controller will be used to handle the approval of claims. Functionality will be added to this controller in the future. */

using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<ApprovalController> _logger;

        public ApprovalController(CMCSDbContext context, ILogger<ApprovalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Verify()
        {
            try
            {
                var claims = await _context.Claims
                    .Include(c => c.User)
                    .Include(c => c.Status)
                    .OrderByDescending(c => c.SubmissionDate)
                    .ToListAsync();

                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving claims for verification");
                return StatusCode(500, "An error occurred while retrieving claims. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid model state", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var claim = await _context.Claims
                    .Include(c => c.User)
                    .Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.ClaimID == id);

                if (claim == null)
                {
                    return NotFound(new { success = false, message = $"Claim with ID {id} not found" });
                }

                var documents = await _context.Documents
                    .Where(d => d.ClaimID == id)
                    .Select(d => d.DocumentName)
                    .ToListAsync();

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