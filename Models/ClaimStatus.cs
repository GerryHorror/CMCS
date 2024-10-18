/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

/* This class will be used to create the ClaimStatus object. This object will be used to store the status of the claim. The status will be stored in the database and will be used to track the status of the claim.
 - StatusID: will be the primary key.
 - StatusName: will be the name of the status (Pending, Approved, Rejected).
 */

using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class ClaimStatus
    {
        [Key]
        public int StatusID { get; set; } // This will be the primary key

        public string StatusName { get; set; }
    }
}