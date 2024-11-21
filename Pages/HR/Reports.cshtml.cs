using CMCS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using CMCS.Reports;
using static CMCS.Models.ReportModel;
using System.Text.Json;

namespace CMCS.Pages.HR
{
    public class ReportsModel : PageModel
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<ReportsModel> _logger;

        [BindProperty]
        public ClaimReportFilter Filter { get; set; } = new();

        public List<string> AvailableStatuses { get; set; } = new();

        public ReportsModel(CMCSDbContext context, ILogger<ReportsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return RedirectToPage("/Index");
            }

            Filter.StartDate = DateTime.Today.AddMonths(-1);
            Filter.EndDate = DateTime.Today;

            await LoadStatuses();
            return Page();
        }

        private async Task LoadStatuses()
        {
            AvailableStatuses = await _context.ClaimStatuses
                .Select(s => s.StatusName)
                .ToListAsync();
            Filter.SelectedStatuses = new List<string>(AvailableStatuses);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostGenerateReportAsync([FromBody] JsonElement jsonElement)
        {
            try
            {
                // Log the received data
                _logger.LogInformation($"Received data: {jsonElement.GetRawText()}");

                // Deserialize the Filter property
                var filterElement = jsonElement.GetProperty("Filter");
                Filter = JsonSerializer.Deserialize<ClaimReportFilter>(filterElement.GetRawText());

                if (!ModelState.IsValid)
                {
                    await LoadStatuses();
                    return Page();
                }

                var claims = await _context.Claims
                    .Include(c => c.User)
                    .Include(c => c.Status)
                    .Where(c => c.SubmissionDate.Date >= Filter.StartDate.Date &&
                               c.SubmissionDate.Date <= Filter.EndDate.Date &&
                               (Filter.SelectedStatuses.Count == 0 ||
                                Filter.SelectedStatuses.Contains(c.Status.StatusName)))
                    .Select(c => new ClaimReportData
                    {
                        LecturerName = $"{c.User.FirstName} {c.User.LastName}",
                        SubmissionDate = c.SubmissionDate,
                        ClaimAmount = c.ClaimAmount,
                        Status = c.Status.StatusName,
                        HoursWorked = c.HoursWorked,
                        HourlyRate = c.HourlyRate,
                        ClaimType = c.ClaimType
                    })
                    .OrderByDescending(c => c.SubmissionDate)
                    .ToListAsync();

                if (!claims.Any())
                {
                    return new JsonResult(new { error = "No claims found for the selected criteria" });
                }

                var document = new ClaimReportDocument(claims, Filter.StartDate, Filter.EndDate);
                var pdfBytes = document.GeneratePdf();

                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Claims_Report_{Filter.StartDate:yyyyMMdd}_to_{Filter.EndDate:yyyyMMdd}.pdf"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating claims report");
                return new JsonResult(new { error = "An error occurred while generating the report" });
            }
        }
    }
}