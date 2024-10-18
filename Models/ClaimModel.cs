/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

/* This model is used to store the claim details of the user. The ClaimID is the primary key and UserID is the foreign key. The ClaimAmount is the amount of the claim, ClaimType is the type of the claim, ClaimStatus is the status of the claim, SubmissionDate is the date when the claim was submitted and ApprovalDate is the date when the claim was approved.
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class ClaimModel
    {
        [Key]
        public int ClaimID { get; set; } // This will be the primary key

        [ForeignKey("User")]
        public int UserID { get; set; } // This will be the foreign key

        [ForeignKey("Status")]
        public int StatusID { get; set; } // This will be the foreign key

        public decimal ClaimAmount { get; set; }
        public string ClaimType { get; set; }

        [Required(ErrorMessage = "Submission date is required")]
        [DataType(DataType.Date)]
        public DateTime SubmissionDate { get; set; }

        public DateTime ApprovalDate { get; set; }
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(150, 350, ErrorMessage = "Hourly rate must be between 150 and 350")]
        public decimal HourlyRate { get; set; }

        // Navigation property (this is used to create a relationship between the Claim and User tables)

        public UserModel? User { get; set; }
        public ClaimStatus? Status { get; set; }
    }
}