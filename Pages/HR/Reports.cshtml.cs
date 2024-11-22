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

        // New properties for invoices
        [BindProperty]
        public int? LecturerId { get; set; }

        [BindProperty]
        public DateTime? InvoiceStartDate { get; set; }

        [BindProperty]
        public DateTime? InvoiceEndDate { get; set; }

        public List<string> AvailableStatuses { get; set; } = new();
        public List<UserModel> Lecturers { get; set; } = new();
        public InvoicePreviewModel? InvoicePreview { get; set; }

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

            await LoadInitialData();
            return Page();
        }

        private async Task LoadInitialData()
        {
            // Load statuses
            AvailableStatuses = await _context.ClaimStatuses
                .Select(s => s.StatusName)
                .ToListAsync();

            // Load lecturers
            Lecturers = await _context.Users
                .Where(u => u.Role.RoleName == "Lecturer")
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            Filter.SelectedStatuses = new List<string>(AvailableStatuses);
        }

        public async Task<IActionResult> OnPostGenerateReportAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadInitialData();
                return Page();
            }

            try
            {
                var claims = await GetFilteredClaims(Filter.StartDate, Filter.EndDate, Filter.SelectedStatuses);

                if (!claims.Any())
                {
                    ModelState.AddModelError(string.Empty, "No claims found for the selected criteria");
                    await LoadInitialData();
                    return Page();
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
                ModelState.AddModelError(string.Empty, "An error occurred while generating the report.");
                await LoadInitialData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostPreviewAsync()
        {
            if (!LecturerId.HasValue || !InvoiceStartDate.HasValue || !InvoiceEndDate.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please select a lecturer and date range");
                await LoadInitialData();
                return Page();
            }

            try
            {
                var lecturer = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserID == LecturerId);

                if (lecturer == null)
                {
                    ModelState.AddModelError(string.Empty, "Selected lecturer not found");
                    await LoadInitialData();
                    return Page();
                }

                var claims = await GetFilteredClaims(
                    InvoiceStartDate.Value,
                    InvoiceEndDate.Value,
                    new[] { "Approved" },
                    LecturerId.Value
                );

                InvoicePreview = new InvoicePreviewModel
                {
                    LecturerName = $"{lecturer.FirstName} {lecturer.LastName}",
                    ClaimCount = claims.Count,
                    TotalAmount = claims.Sum(c => c.ClaimAmount),
                    Claims = claims,
                    LecturerDetails = lecturer
                };

                await LoadInitialData();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice preview");
                ModelState.AddModelError(string.Empty, "An error occurred while generating the preview");
                await LoadInitialData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostGenerateInvoiceAsync()
        {
            if (!LecturerId.HasValue || !InvoiceStartDate.HasValue || !InvoiceEndDate.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please select a lecturer and date range");
                await LoadInitialData();
                return Page();
            }

            try
            {
                var lecturer = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserID == LecturerId);

                if (lecturer == null)
                {
                    ModelState.AddModelError(string.Empty, "Selected lecturer not found");
                    await LoadInitialData();
                    return Page();
                }

                var claims = await GetFilteredClaims(
                    InvoiceStartDate.Value,
                    InvoiceEndDate.Value,
                    new[] { "Approved" },
                    LecturerId.Value
                );

                if (!claims.Any())
                {
                    ModelState.AddModelError(string.Empty, "No approved claims found for the selected period");
                    await LoadInitialData();
                    return Page();
                }

                var invoiceData = new InvoiceModel
                {
                    Lecturer = lecturer,
                    StartDate = InvoiceStartDate.Value,
                    EndDate = InvoiceEndDate.Value,
                    InvoiceNumber = GenerateInvoiceNumber(),
                    InvoiceDate = DateTime.Now,
                    Claims = claims
                };

                var document = new InvoiceDocument(invoiceData);
                var pdfBytes = document.GeneratePdf();

                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Invoice_{lecturer.LastName}_{InvoiceStartDate:yyyyMMdd}.pdf"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice");
                ModelState.AddModelError(string.Empty, "An error occurred while generating the invoice");
                await LoadInitialData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostGenerateAllInvoicesAsync()
        {
            if (!InvoiceStartDate.HasValue || !InvoiceEndDate.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please select a date range");
                await LoadInitialData();
                return Page();
            }

            try
            {
                var lecturers = await _context.Users
                    .Where(u => u.Role.RoleName == "Lecturer")
                    .ToListAsync();

                using var memoryStream = new MemoryStream();
                using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var lecturer in lecturers)
                    {
                        var claims = await GetFilteredClaims(
                            InvoiceStartDate.Value,
                            InvoiceEndDate.Value,
                            new[] { "Approved" },
                            lecturer.UserID
                        );

                        if (claims.Any())
                        {
                            var invoiceData = new InvoiceModel
                            {
                                Lecturer = lecturer,
                                StartDate = InvoiceStartDate.Value,
                                EndDate = InvoiceEndDate.Value,
                                InvoiceNumber = GenerateInvoiceNumber(),
                                InvoiceDate = DateTime.Now,
                                Claims = claims
                            };

                            var document = new InvoiceDocument(invoiceData);
                            var pdfBytes = document.GeneratePdf();

                            var entry = archive.CreateEntry($"Invoice_{lecturer.LastName}_{InvoiceStartDate:yyyyMMdd}.pdf");
                            using var entryStream = entry.Open();
                            await entryStream.WriteAsync(pdfBytes);
                        }
                    }
                }

                return File(
                    memoryStream.ToArray(),
                    "application/zip",
                    $"Invoices_{InvoiceStartDate:yyyyMMdd}_to_{InvoiceEndDate:yyyyMMdd}.zip"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoices");
                ModelState.AddModelError(string.Empty, "An error occurred while generating the invoices");
                await LoadInitialData();
                return Page();
            }
        }

        private async Task<List<ClaimReportData>> GetFilteredClaims(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<string> statuses,
            int? lecturerId = null)
        {
            var query = _context.Claims
                .Include(c => c.User)
                .Include(c => c.Status)
                .Where(c => c.SubmissionDate.Date >= startDate.Date &&
                           c.SubmissionDate.Date <= endDate.Date &&
                           statuses.Contains(c.Status.StatusName));

            if (lecturerId.HasValue)
            {
                query = query.Where(c => c.UserID == lecturerId.Value);
            }

            return await query
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
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class InvoicePreviewModel
    {
        public string LecturerName { get; set; }
        public int ClaimCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ClaimReportData> Claims { get; set; }
        public UserModel LecturerDetails { get; set; }
    }
}