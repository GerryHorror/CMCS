/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

/* This controller is used to handle the approval of claims. It includes a method to retrieve claims for verification and a method to update the status of a claim. */

using CMCS.Data;
using CMCS.Models;
using CMCS.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ApprovalController : Controller
    {
        // Declare the database context and logger

        private readonly CMCSDbContext _context;
        private readonly ILogger<ApprovalController> _logger;
        private readonly ClaimProcessor _claimProcessor;

        // Constructor to initialise the database context and logger
        public ApprovalController(CMCSDbContext context, ILogger<ApprovalController> logger)
        {
            _context = context;
            _logger = logger;
            var validator = new ClaimValidator(); // Assuming ClaimValidator has a parameterless constructor
            _claimProcessor = new ClaimProcessor(validator, context);
        }

        // Method to retrieve claims for verification and return the view
        public async Task<IActionResult> Verify()
        {
            try
            {
                // Retrieve claims from the database
                var claims = await _context.Claims
                    .Include(c => c.User)
                    .Include(c => c.Status)
                    .OrderByDescending(c => c.SubmissionDate)
                    .ToListAsync();

                // Check each pending claim for auto-approval
                foreach (var claim in claims.Where(c => c.Status.StatusName == "Pending"))
                {
                    var (success, message, autoApproved) = await _claimProcessor.ProcessClaimAsync(claim);
                    if (autoApproved)
                    {
                        _logger.LogInformation("Claim {ClaimId} was auto-approved", claim.ClaimID);
                    }
                }

                // Refresh claims after potential auto-approvals
                claims = await _context.Claims
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

        // <-------------------------------------------------------------------------------------------------------------------------------------------->

        // Method to update the status of a claim and return a success message
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid model state" });
            }

            try
            {
                var claim = await _context.Claims
                    .Include(c => c.User)
                    .Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.ClaimID == model.ClaimId);

                if (claim == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Claim with ID {model.ClaimId} not found"
                    });
                }

                // Process claim for auto-approval
                var (success, message, autoApproved) = await _claimProcessor.ProcessClaimAsync(claim);

                if (!success)
                {
                    return BadRequest(new { success = false, message });
                }

                // If not auto-approved and manual approval requested, process manual approval
                if (!autoApproved && model.Status != null)
                {
                    var newStatus = await _context.ClaimStatuses
                        .FirstOrDefaultAsync(s => s.StatusName == model.Status);

                    if (newStatus == null)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = $"Invalid status: {model.Status}"
                        });
                    }

                    claim.StatusID = newStatus.StatusID;
                    claim.ApprovalDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    message = $"Claim {model.ClaimId} has been manually {model.Status.ToLower()}.";
                }

                _logger.LogInformation("Claim {ClaimId} processed. Auto-approved: {AutoApproved}",
                    model.ClaimId, autoApproved);

                return Json(new
                {
                    success = true,
                    message = message,
                    isAutoApproved = autoApproved
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing claim {ClaimId}", model.ClaimId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while processing the claim."
                });
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