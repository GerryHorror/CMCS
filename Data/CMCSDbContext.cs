/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

/*
The CMCSDbContext class is responsible for managing the database context for the CMCS application.
It inherits from the DbContext class provided by Entity Framework Core, which is an Object-Relational Mapper
(ORM) that enables .NET developers to work with a database using .NET objects.This class is used to configure
the database connection, define the DbSet properties representing the tables in the database, and
manage the interactions with the database.
*/

using Microsoft.EntityFrameworkCore;
using CMCS.Models;

namespace CMCS.Data
{
    public class CMCSDbContext : DbContext
    {
        // Constructor to initialise the DbContext with options
        public CMCSDbContext(DbContextOptions<CMCSDbContext> options) : base(options)
        {
        }

        // DbSet properties to access the database tables

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ClaimModel> Claims { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<ClaimStatus> ClaimStatuses { get; set; }
    }
}