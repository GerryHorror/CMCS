/* Boiler plate code for the Document Model. This model is used to store the document details of the user. The DocumentID is the primary key, UserID is the foreign key, ClaimID is the foreign key, DocumentName is the name of the document, DocumentType is the type of the document, DocumentStatus is the status of the document, SubmissionDate is the date when the document was submitted and ApprovalDate is the date when the document was approved. Functionality will be added to this model in the future. */

namespace CMCS.Models
{
    public class DocumentModel
    {
        public int DocumentID { get; set; }
        public int UserID { get; set; }
        public int ClaimID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentStatus { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime ApprovalDate { get; set; }
    }
}