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

        public ApprovalController(CMCSDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Verify()
        {
            var claims = await _context.Claims.Include(c => c.User).Include(c => c.Status).OrderByDescending(c => c.SubmissionDate).ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusModel model)
        {
            var claim = await _context.Claims.FindAsync(model.ClaimId);
            if (claim == null)
            {
                return NotFound();
            }

            var newStatus = await _context.ClaimStatuses.FirstOrDefaultAsync(s => s.StatusName == model.Status);
            if (newStatus == null)
            {
                return BadRequest("Invalid status");
            }

            claim.StatusID = newStatus.StatusID;
            if (model.Status == "Approved" || model.Status == "Rejected")
            {
                claim.ApprovalDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Claim {model.ClaimId} has been {model.Status.ToLower()}." });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _context.Claims.Include(c => c.User).Include(c => c.Status).FirstOrDefaultAsync(c => c.ClaimID == id);

            if (claim == null)
            {
                return NotFound();
            }

            var documents = await _context.Documents.Where(d => d.ClaimID == id).Select(d => d.DocumentName).ToListAsync();

            return Json(new
            {
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
    }
}