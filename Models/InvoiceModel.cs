using static CMCS.Models.ReportModel;

namespace CMCS.Models
{
    public class InvoiceModel
    {
        public UserModel Lecturer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<ClaimReportData> Claims { get; set; }
        public decimal TotalAmount => Claims.Sum(c => c.ClaimAmount);

        // Optional: Additional properties for invoice customisation

        public string InstitutionName { get; set; } = "The Independent Institute of Education";
        public string InstitutionAddress { get; set; } = "123 University Street";
        public string InstitutionCity { get; set; } = "City, State, ZIP";
        public string PaymentTerms { get; set; } = "Payment due within 30 days";
    }
}