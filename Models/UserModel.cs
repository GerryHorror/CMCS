/* Boiler plate code for the User Model. This model is used to store the user details. The UserID is the primary key, UserName is the name of the user, FirstName is the first name of the user, LastName is the last name of the user, UserEmail is the email of the user, PhoneNumber is the phone number of the user, BankDetails is the bank details of the user, BankAccountNumber is the bank account number of the user, Address is the address of the user and UserPassword is the password of the user. Functionality will be added to this model in the future.
 */

namespace CMCS.Models
{
    public class UserModel
    {
        public int UserID { get; set; } // This will be the primary key
        public int RoleID { get; set; } // This will be the foreign key
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public string PhoneNumber { get; set; } // Changed from int to string because South African phone numbers start with 0, and to allow users to enter the country code if they have an international number
        public string BankName { get; set; }
        public string BranchCode { get; set; } // Standard Bank branch code starts with 0
        public int BankAccountNumber { get; set; }
        public string Address { get; set; }
        public string UserPassword { get; set; }
    }
}