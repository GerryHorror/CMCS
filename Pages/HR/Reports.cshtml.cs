using CMCS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using CMCS.Reports;
using static CMCS.Models.ReportModel;
using System.Text.Json;
using CMCS.Models;

namespace CMCS.Pages.HR
{
    public class ReportsModel : PageModel
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<ReportsModel> _logger;

        [BindProperty]
        public ClaimReportFilter Filter { get; set; } = new();

        [BindProperty]
        public List<UserModel> Lecturers { get; set; } = new();

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

            // Load lecturers
            Lecturers = await _context.Users
                .Where(u => u.RoleID == 1)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

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
                var filterElement = jsonElement.GetProperty("Filter");
                Filter = JsonSerializer.Deserialize<ClaimReportFilter>(filterElement.GetRawText());

                if (!ModelState.IsValid)
                {
                    return new JsonResult(new { error = "Invalid data received" });
                }

                var claims = await _context.Claims
                    .Include(c => c.User)
                    .Include(c => c.Status)
                    .Where(c => c.SubmissionDate.Date >= Filter.StartDate.Date &&
                               c.SubmissionDate.Date <= Filter.EndDate.Date &&
                               (Filter.SelectedStatuses.Count == 0 || Filter.SelectedStatuses.Contains(c.Status.StatusName)))
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

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostGenerateInvoiceAsync([FromBody] JsonElement jsonElement)
        {
            try
            {
                var filterElement = jsonElement.GetProperty("Filter");
                Filter = JsonSerializer.Deserialize<ClaimReportFilter>(filterElement.GetRawText());
                var lecturerIdStr = jsonElement.GetProperty("LecturerId").GetString();
                if (!int.TryParse(lecturerIdStr, out int lecturerId))
                {
                    return new JsonResult(new { error = "Invalid lecturer ID" });
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var claims = await _context.Claims
                    .Include(c => c.User)
                    .Where(c => c.UserID == lecturerId &&
                               c.SubmissionDate.Date >= Filter.StartDate.Date &&
                               c.SubmissionDate.Date <= Filter.EndDate.Date &&
                               c.Status.StatusName == "Approved")
                    .Select(c => new ClaimReportData
                    {
                        LecturerName = $"{c.User.FirstName} {c.User.LastName}",
                        SubmissionDate = c.SubmissionDate,
                        ClaimAmount = c.ClaimAmount,
                        HoursWorked = c.HoursWorked,
                        HourlyRate = c.HourlyRate,
                        ClaimType = c.ClaimType
                    })
                    .OrderByDescending(c => c.SubmissionDate)
                    .ToListAsync();

                if (!claims.Any())
                {
                    return new JsonResult(new { error = "No approved claims found for the selected lecturer in this date range" });
                }

                var lecturer = await _context.Users.FindAsync(lecturerId);

                // In your ReportsModel.cs
                var invoiceModel = new InvoiceModel
                {
                    // The lecturer is now the one issuing the invoice
                    CompanyName = $"{lecturer.FirstName} {lecturer.LastName} - Independent Contractor",
                    CompanyAddress = lecturer.Address,
                    CompanyContact = lecturer.PhoneNumber,
                    CompanyEmail = lecturer.UserEmail,
                    BankDetails = new BankDetails
                    {
                        BankName = lecturer.BankName,
                        AccountNumber = lecturer.BankAccountNumber,
                        BranchCode = lecturer.BranchCode
                    },

                    // The institution is now being invoiced
                    BillTo = new BillingDetails
                    {
                        Name = "Southern Hemisphere Institute of Technology",
                        Address = "123 University Street",
                        City = "Johannesburg",
                        PostalCode = "2000",
                        Country = "South Africa"
                    },

                    StartDate = Filter.StartDate,
                    EndDate = Filter.EndDate,
                    InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}",
                    InvoiceDate = DateTime.Now,
                    Claims = claims,
                    PaymentTerms = "Payment due within 30 days",
                    VAT = "Not VAT Registered" // or lecturer's VAT number if applicable
                };

                var document = new InvoiceDocument(invoiceModel);
                var pdfBytes = document.GeneratePdf();

                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Invoice_{lecturer.FirstName}_{lecturer.LastName}_{Filter.StartDate:yyyyMMdd}.pdf"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice");
                return new JsonResult(new { error = "An error occurred while generating the invoice" });
            }
        }
    }
}