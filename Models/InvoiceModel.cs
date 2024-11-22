using static CMCS.Models.ReportModel;

namespace CMCS.Models
{
    public class InvoiceModel
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string CompanyEmail { get; set; }
        public BankDetails BankDetails { get; set; }
        public BillingDetails BillTo { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ClaimReportData> Claims { get; set; }
        public string PaymentTerms { get; set; }
        public string VAT { get; set; }

        public decimal TotalAmount => Claims?.Sum(c => c.ClaimAmount) ?? 0;
    }

    public class BankDetails
    {
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
    }

    public class BillingDetails
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}