/* Boiler plate code for the Claim Model. This model is used to store the claim details of the user. The ClaimID is the primary key and UserID is the foreign key. The ClaimAmount is the amount of the claim, ClaimType is the type of the claim, ClaimStatus is the status of the claim, SubmissionDate is the date when the claim was submitted and ApprovalDate is the date when the claim was approved. Functionality will be added to this model in the future. */

namespace CMCS.Models
{
    public class ClaimModel
    {
        public int ClaimID { get; set; }
        public int UserID { get; set; }
        public decimal ClaimAmount { get; set; }
        public string ClaimType { get; set; }
        public string ClaimStatus { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime ApprovalDate { get; set; }
    }
}