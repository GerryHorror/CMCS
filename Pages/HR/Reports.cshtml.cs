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
        // Properties for the CMCSDbContext and ILogger

        private readonly CMCSDbContext _context;
        private readonly ILogger<ReportsModel> _logger;

        // Bind properties for the ClaimReportFilter and Lecturers (List of UserModel)

        [BindProperty]
        public ClaimReportFilter Filter { get; set; } = new();

        [BindProperty]
        public List<UserModel> Lecturers { get; set; } = new();

        // List of available statuses
        public List<string> AvailableStatuses { get; set; } = new();

        public ReportsModel(CMCSDbContext context, ILogger<ReportsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        // This method is used to handle the GET request for the Reports page. It checks the user role and loads the lecturers and statuses.
        public async Task<IActionResult> OnGetAsync()
        {
            // Check user role and redirect if not Coordinator or Manager
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return RedirectToPage("/Index");
            }

            // Set default date range for the filter
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

        // <-------------------------------------------------------------------------------------->

        // This method is used to handle the POST request for the Reports page. It generates the claims report based on the filter criteria.
        private async Task LoadStatuses()
        {
            AvailableStatuses = await _context.ClaimStatuses
                .Select(s => s.StatusName)
                .ToListAsync();
            Filter.SelectedStatuses = new List<string>(AvailableStatuses);
        }

        // <-------------------------------------------------------------------------------------->

        // This method is used to handle the POST request for the Reports page. It generates the claims report based on the filter criteria.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostGenerateReportAsync([FromBody] JsonElement jsonElement)
        {
            try
            {
                // Deserialize the JSON data received from the client
                var filterElement = jsonElement.GetProperty("Filter");
                Filter = JsonSerializer.Deserialize<ClaimReportFilter>(filterElement.GetRawText());

                if (!ModelState.IsValid)
                {
                    return new JsonResult(new { error = "Invalid data received" });
                }

                // Retrieve claims based on the filter criteria
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
                // Generate the claims report document
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

        // <-------------------------------------------------------------------------------------->

        // This method is used to handle the POST request for the Reports page. It generates the invoice based on the filter criteria.
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

                var invoiceModel = new InvoiceModel
                {
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