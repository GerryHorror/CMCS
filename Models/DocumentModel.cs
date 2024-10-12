/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

/* Boiler plate code for the Document Model. This model is used to store the document details of the user. The DocumentID is the primary key, UserID is the foreign key, ClaimID is the foreign key, DocumentName is the name of the document, DocumentType is the type of the document, DocumentStatus is the status of the document, SubmissionDate is the date when the document was submitted and ApprovalDate is the date when the document was approved. Functionality will be added to this model in the future.
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class DocumentModel
    {
        [Key]
        public int DocumentID { get; set; } // This will be the primary key

        [ForeignKey("Claim")]
        public int ClaimID { get; set; } // This will be the foreign key

        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public DateTime UploadDate { get; set; }

        // Navigation property (this is used to create a relationship between the Document and User tables)
        public ClaimModel Claim { get; set; }
    }
}