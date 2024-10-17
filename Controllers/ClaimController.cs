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
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Home");
            }

            var claims = await _context.Claims.Where(c => c.UserID == userId.Value).Include(c => c.Status).OrderByDescending(c => c.SubmissionDate).ToListAsync();

            var claimIds = claims.Select(c => c.ClaimID).ToList();
            var documents = await _context.Documents.Where(d => claimIds.Contains(d.ClaimID)).ToListAsync();

            var viewModel = new ClaimViewModel
            {
                Claims = claims,
                Documents = documents.GroupBy(d => d.ClaimID).ToDictionary(g => g.Key, g => g.ToList())
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Home");
            }

            var claim = await _context.Claims.Include(c => c.Status).FirstOrDefaultAsync(c => c.ClaimID == id && c.UserID == userId.Value);

            if (claim == null)
            {
                return NotFound();
            }

            var documents = await _context.Documents.Where(d => d.ClaimID == id).ToListAsync();

            return Json(new
            {
                claim.ClaimID,
                claim.SubmissionDate,
                claim.ClaimAmount,
                claim.Status.StatusName,
                claim.HoursWorked,
                claim.HourlyRate,
                claim.ClaimType,
                Documents = documents.Select(d => d.DocumentName).ToList()
            });
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

                    // Validate work entries
                    foreach (var entry in workEntries)
                    {
                        if (entry.WorkDate > claimModel.SubmissionDate)
                        {
                            ModelState.AddModelError("", "Work date cannot be after the submission date.");
                            return View(claimModel);
                        }
                        if (entry.HoursWorked < 1 || entry.HoursWorked > 8)
                        {
                            ModelState.AddModelError("", "Hours worked must be between 1 and 8.");
                            return View(claimModel);
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