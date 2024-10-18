/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

// This class will be used to create the UpdateStatusModel object. This object will be used to store the status of the claim. The status will be stored in the database and will be used to track the status of the claim.

namespace CMCS.Models
{
    public class UpdateStatusModel
    {
        public int ClaimId { get; set; }
        public string Status { get; set; }
    }
}