using CMCS.Data;
using CMCS.Models;

namespace CMCS
{
    public class DatabaseSeeder
    {
        public static void SeedRoles(CMCSDbContext context)
        {
            if (!context.Roles.Any())
            {
                // Add roles without specifying RoleID
                context.Roles.AddRange(
                    new RoleModel { RoleName = "Lecturer" },
                    new RoleModel { RoleName = "Coordinator" },
                    new RoleModel { RoleName = "Manager" }
                );

                context.SaveChanges();
            }
        }

        public static void SeedClaimStatuses(CMCSDbContext context)
        {
            if (!context.ClaimStatuses.Any())
            {
                // Add claim statuses without specifying StatusID
                context.ClaimStatuses.AddRange(
                    new ClaimStatus { StatusName = "Pending" },
                    new ClaimStatus { StatusName = "Approved" },
                    new ClaimStatus { StatusName = "Rejected" }
                );
                context.SaveChanges();
            }
        }

        // This method is called from Program.cs. It is used for seeding the database with users. If the Users table is empty, it will add users to the table.
        public static void SeedUsers(CMCSDbContext context)
        {
            if (!context.Users.Any())
            {
                var lecturerRole = context.Roles.FirstOrDefault(r => r.RoleName == "Lecturer");
                var coordinatorRole = context.Roles.FirstOrDefault(r => r.RoleName == "Coordinator");
                var managerRole = context.Roles.FirstOrDefault(r => r.RoleName == "Manager");

                if (lecturerRole != null && coordinatorRole != null && managerRole != null)
                {
                    context.Users.AddRange(
                        new UserModel
                        {
                            UserName = "sipho.nkosi",
                            FirstName = "Sipho",
                            LastName = "Nkosi",
                            UserEmail = "sipho.nkosi@university.ac.za",
                            PhoneNumber = "0721234567",
                            BankName = "FNB",
                            BranchCode = "250655",
                            BankAccountNumber = "62123456789",
                            Address = "123 Mandela Street, Soweto, Johannesburg",
                            UserPassword = "password",
                            RoleID = lecturerRole.RoleID
                        },
                        new UserModel
                        {
                            UserName = "fatima.patel",
                            FirstName = "Fatima",
                            LastName = "Patel",
                            UserEmail = "fatima.patel@university.ac.za",
                            PhoneNumber = "083 987 6543",
                            BankName = "Standard Bank",
                            BranchCode = "051001",
                            BankAccountNumber = "0012345678912",
                            Address = "45 Long Street, Cape Town",
                            UserPassword = "password",
                            RoleID = coordinatorRole.RoleID
                        },
                        new UserModel
                        {
                            UserName = "johan.vandermerwe",
                            FirstName = "Johan",
                            LastName = "van der Merwe",
                            UserEmail = "johan.vandermerwe@university.ac.za",
                            PhoneNumber = "061 555 7890",
                            BankName = "Nedbank",
                            BranchCode = "198765",
                            BankAccountNumber = "1122334455",
                            Address = "789 Church Street, Pretoria",
                            UserPassword = "password",
                            RoleID = managerRole.RoleID
                        },
                        new UserModel
                        {
                            UserName = "zanele.mbeki",
                            FirstName = "Zanele",
                            LastName = "Mbeki",
                            UserEmail = "zanele.mbeki@university.ac.za",
                            PhoneNumber = "084 321 9876",
                            BankName = "Absa",
                            BranchCode = "632005",
                            BankAccountNumber = "4055123456",
                            Address = "22 Maphuta Street, Durban",
                            UserPassword = "password",
                            RoleID = lecturerRole.RoleID
                        },
                        new UserModel
                        {
                            UserName = "david.oconnor",
                            FirstName = "David",
                            LastName = "O'Connor",
                            UserEmail = "david.oconnor@university.ac.za",
                            PhoneNumber = "071 234 5678",
                            BankName = "Capitec",
                            BranchCode = "470010",
                            BankAccountNumber = "1234567890",
                            Address = "5 Baobab Avenue, Polokwane",
                            UserPassword = "password",
                            RoleID = coordinatorRole.RoleID
                        }
                    );

                    context.SaveChanges();
                }
            }
        }

        // This method is called from Program.cs. It is used for seeding the database with roles and claim statuses.
        public static void Initialise(CMCSDbContext context)
        {
            SeedRoles(context);
            SeedClaimStatuses(context);
            SeedUsers(context);
        }
    }
}