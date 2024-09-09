/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

/* Boiler plate code for the Claim Model. This model is used to store the claim details of the user. The ClaimID is the primary key and UserID is the foreign key. The ClaimAmount is the amount of the claim, ClaimType is the type of the claim, ClaimStatus is the status of the claim, SubmissionDate is the date when the claim was submitted and ApprovalDate is the date when the claim was approved. Functionality will be added to this model in the future. */

namespace CMCS.Models
{
    public class ClaimModel
    {
        public int ClaimID { get; set; } // This will be the primary key
        public int UserID { get; set; } // This will be the foreign key
        public int StatusID { get; set; } // This will be the foreign key
        public decimal ClaimAmount { get; set; }
        public string ClaimType { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
    }
}