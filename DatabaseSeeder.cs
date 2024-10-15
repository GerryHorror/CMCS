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
    }
}