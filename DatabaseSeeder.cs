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

        // TODO: Seed users for testing purposes
        public static void SeedUsers(CMCSDbContext context)
        {
        }

        // This method is called from Program.cs. It is used for seeding the database with roles and claim statuses.
        public static void Initialise(CMCSDbContext context)
        {
            SeedRoles(context);
            SeedClaimStatuses(context);
        }
    }
}