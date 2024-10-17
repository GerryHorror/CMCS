/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

/* This is the boiler plate code for the Claim Controller. This controller will be used to handle the claims. Functionality will be added to this controller in the future. */

using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Controllers
{
    public class ClaimController : Controller
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<ClaimController> _logger;

        public ClaimController(CMCSDbContext context, ILogger<ClaimController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Claim()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorised access attempt to Claim action");
                    return RedirectToAction("Login", "Home");
                }

                var claims = await _context.Claims
                    .Where(c => c.UserID == userId.Value)
                    .Include(c => c.Status)
                    .OrderByDescending(c => c.SubmissionDate)
                    .ToListAsync();

                var claimIds = claims.Select(c => c.ClaimID).ToList();
                var documents = await _context.Documents
                    .Where(d => claimIds.Contains(d.ClaimID))
                    .ToListAsync();

                var viewModel = new ClaimViewModel
                {
                    Claims = claims,
                    Documents = documents.GroupBy(d => d.ClaimID).ToDictionary(g => g.Key, g => g.ToList())
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving claims for user");
                return StatusCode(500, "An error occurred while retrieving your claims. Please try again later.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorised access attempt to Details action");
                    return RedirectToAction("Login", "Home");
                }

                var claim = await _context.Claims
                    .Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.ClaimID == id && c.UserID == userId.Value);

                if (claim == null)
                {
                    _logger.LogWarning("Attempt to access non-existent or unauthorised claim: {ClaimId}", id);
                    return NotFound(new { success = false, message = "Claim not found or access denied" });
                }

                var documents = await _context.Documents
                    .Where(d => d.ClaimID == id)
                    .Select(d => d.DocumentName)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    claim.ClaimID,
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

        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ClaimModel claimModel, List<WorkEntry> workEntries, IFormFile supportingDocument)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Invalid model state in Submit action. Errors: {Errors}", string.Join(", ", errors));
                return Json(new { success = false, message = "Invalid data", errors = errors });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorised claim submission attempt");
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // File validation
                if (supportingDocument != null)
                {
                    // Check file size (e.g., 5MB limit)
                    if (supportingDocument.Length > 5 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "File size exceeds 5MB limit" });
                    }

                    // Check file type
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var fileExtension = Path.GetExtension(supportingDocument.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return Json(new { success = false, message = "Only .pdf, .docx, and .xlsx files are allowed" });
                    }
                }

                claimModel.UserID = userId.Value;
                claimModel.SubmissionDate = DateTime.Now;
                claimModel.StatusID = await _context.ClaimStatuses
                    .Where(s => s.StatusName == "Pending")
                    .Select(s => s.StatusID)
                    .FirstOrDefaultAsync();

                // Validate work entries
                foreach (var entry in workEntries)
                {
                    if (entry.WorkDate > claimModel.SubmissionDate)
                    {
                        ModelState.AddModelError("", "Work date cannot be after the submission date.");
                        return Json(new { success = false, message = "Invalid work entry date" });
                    }
                    if (entry.HoursWorked < 1 || entry.HoursWorked > 8)
                    {
                        ModelState.AddModelError("", "Hours worked must be between 1 and 8.");
                        return Json(new { success = false, message = "Invalid work hours" });
                    }
                }

                // Calculate total hours worked and claim amount
                claimModel.HoursWorked = workEntries.Sum(w => w.HoursWorked);
                claimModel.ClaimAmount = claimModel.HoursWorked * claimModel.HourlyRate;

                _context.Claims.Add(claimModel);
                await _context.SaveChangesAsync();

                // Handle file upload
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await supportingDocument.CopyToAsync(memoryStream);

                    var document = new DocumentModel
                    {
                        ClaimID = claimModel.ClaimID,
                        DocumentName = supportingDocument.FileName,
                        DocumentType = supportingDocument.ContentType,
                        UploadDate = DateTime.Now,
                        FileContent = memoryStream.ToArray()
                    };

                    _context.Documents.Add(document);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Claim {ClaimId} submitted successfully", claimModel.ClaimID);
                return Json(new { success = true, message = "Claim submitted successfully" });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error occurred while submitting claim");
                return Json(new { success = false, message = "A database error occurred while submitting the claim", error = ex.Message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error occurred while submitting claim");
                return Json(new { success = false, message = "An unexpected error occurred while submitting the claim", error = ex.Message });
            }
        }
    }
}