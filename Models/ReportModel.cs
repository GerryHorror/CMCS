using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class ReportModel
    {
        public class ClaimReportFilter
        {
            [Required(ErrorMessage = "Start date is required")]
            [Display(Name = "Start Date")]
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "End date is required")]
            [Display(Name = "End Date")]
            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }

            public List<string> SelectedStatuses { get; set; } = new();
        }

        public class ClaimReportData
        {
            public string LecturerName { get; set; }
            public DateTime SubmissionDate { get; set; }
            public decimal ClaimAmount { get; set; }
            public string Status { get; set; }
            public decimal HoursWorked { get; set; }
            public decimal HourlyRate { get; set; }
            public string ClaimType { get; set; }
        }

        public class ClaimSummary
        {
            public int TotalClaims { get; set; }
            public decimal TotalAmount { get; set; }
            public Dictionary<string, int> ClaimsByStatus { get; set; } = new();
            public Dictionary<string, decimal> AmountByStatus { get; set; } = new();
        }
    }
}