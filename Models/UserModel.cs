/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

/*This model is used to store the user details. The UserID is the primary key, UserName is the name of the user, FirstName is the first name of the user, LastName is the last name of the user, UserEmail is the email of the user, PhoneNumber is the phone number of the user, BankDetails is the bank details of the user, BankAccountNumber is the bank account number of the user, Address is the address of the user and UserPassword is the password of the user.
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; } // This will be the primary key

        [Required(ErrorMessage = "Role is required")]
        [ForeignKey("Role")]
        public int RoleID { get; set; } // This will be the foreign key

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s-]*$", ErrorMessage = "First name can only contain letters, spaces, and hyphens")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s-]*$", ErrorMessage = "Last name can only contain letters, spaces, and hyphens")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string UserEmail { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } // Changed from int to string because South African phone numbers start with 0, and to allow users to enter the country code if they have an international number

        [Required(ErrorMessage = "Bank is required")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Branch code is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Branch code must be 6 digits")]
        public string BranchCode { get; set; } // Standard Bank branch code starts with 0

        [Required(ErrorMessage = "Bank account number is required")]
        [RegularExpression(@"^\d{9,12}$", ErrorMessage = "Bank account number must be between 9 and 12 digits")]
        public string BankAccountNumber { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string UserPassword { get; set; }

        // Navigation property (this is used to create a relationship between the User and Role tables)
        public virtual RoleModel? Role { get; set; }
    }
}