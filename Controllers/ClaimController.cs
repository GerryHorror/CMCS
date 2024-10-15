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
        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ClaimModel claimModel, List<WorkEntry> workEntries, IFormFile supportingDocument)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                _logger.LogError("ModelState is invalid. Errors: {Errors}", string.Join(", ", errors));
                return Json(new { success = false, message = "Invalid data", errors = errors });
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var userId = HttpContext.Session.GetInt32("UserID");
                    if (!userId.HasValue)
                    {
                        return Json(new { success = false, message = "User not authenticated" });
                    }

                    claimModel.UserID = userId.Value;
                    claimModel.SubmissionDate = DateTime.Now;
                    claimModel.StatusID = await _context.ClaimStatuses.Where(s => s.StatusName == "Pending").Select(s => s.StatusID).FirstOrDefaultAsync();

                    // Calculate total hours worked and claim amount
                    decimal totalHours = workEntries.Sum(w => w.HoursWorked);
                    claimModel.HoursWorked = totalHours;
                    claimModel.ClaimAmount = totalHours * claimModel.HourlyRate;

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
                    return Json(new { success = true, message = "Claim submitted successfully" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occurred while submitting claim");
                    return Json(new { success = false, message = "An error occurred while submitting the claim", error = ex.Message });
                }
            }

            return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
    }
}