using Microsoft.EntityFrameworkCore;
using CMCS.Models;

namespace CMCS.Data
{
    public class CMCSDbContext : DbContext
    {
        public CMCSDbContext(DbContextOptions<CMCSDbContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ClaimModel> Claims { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<ClaimStatus> ClaimStatuses { get; set; }
    }
}