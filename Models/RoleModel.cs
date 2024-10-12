/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

/* This class will be used to define the RoleModel object. This object will be used to define the Role object. The Role object will be used to define the Role table in the database. The Role table will be used to store the different roles that a user can have. The Role table will have the following columns:
 - RoleID: This will be the primary key of the table.
 - RoleName: This will be the name of the role (Lecturer, Program Coordinator, Academic Manager).
 */

using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class RoleModel
    {
        [Key]
        public int RoleID { get; set; } // This will be the primary key

        public string RoleName { get; set; }
    }
}