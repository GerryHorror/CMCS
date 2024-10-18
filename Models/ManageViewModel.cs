/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

// This class will be used to define the ManageViewModel object. This object will be used to define the ManageViewModel object. The ManageViewModel object will be used to define the ManageViewModel table in the database. The ManageViewModel table will be used to store the different users and roles that a user can have. The ManageViewModel table will have the following columns:
//  - Users: This will be a list of users.
//  - Roles: This will be a list of roles.

namespace CMCS.Models
{
    public class ManageViewModel
    {
        public List<UserModel> Users { get; set; }
        public List<RoleModel> Roles { get; set; }
    }
}