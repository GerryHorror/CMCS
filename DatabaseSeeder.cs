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
                context.ClaimStatuses.AddRange(
                    new ClaimStatus { StatusName = "Pending" },
                    new ClaimStatus { StatusName = "Approved" },
                    new ClaimStatus { StatusName = "Rejected" }
                );
                context.SaveChanges();
            }
        }

        public static void Initialise(CMCSDbContext context)
        {
            SeedRoles(context);
            SeedClaimStatuses(context);
        }
    }
}