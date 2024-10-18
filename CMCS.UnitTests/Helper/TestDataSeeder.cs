using CMCS.Data;
using CMCS.Models;

namespace CMCS.UnitTests.Helper
{
    public static class TestDataSeeder
    {
        public static void SeedTestData(CMCSDbContext context)
        {
            try
            {
                // Clear existing data to avoid conflicts
                context.Claims.RemoveRange(context.Claims);
                context.Users.RemoveRange(context.Users);
                context.ClaimStatuses.RemoveRange(context.ClaimStatuses);
                context.SaveChanges();

                // Seed ClaimStatus if not already seeded by DatabaseSeeder
                if (!context.ClaimStatuses.Any())
                {
                    context.ClaimStatuses.AddRange(
                        new ClaimStatus { StatusName = "Pending" },
                        new ClaimStatus { StatusName = "Approved" },
                        new ClaimStatus { StatusName = "Rejected" }
                    );
                }

                // Seed UserModel with required properties
                var user = new UserModel
                {
                    UserID = 1,
                    UserName = "JohnDoe",
                    FirstName = "John",
                    LastName = "Doe",
                    UserEmail = "john.doe@example.com",
                    PhoneNumber = "0123456789",
                    Address = "123 Main St",
                    BankAccountNumber = "1234567890",
                    BankName = "Bank A",
                    BranchCode = "123456",
                    UserPassword = "Password123"
                };

                // Seed ClaimModel with relationships
                var claims = new List<ClaimModel>
            {
                new ClaimModel
                {
                    ClaimID = 1,
                    ClaimAmount = 500.00m,
                    SubmissionDate = DateTime.Now,
                    ClaimType = "Service",
                    HoursWorked = 5,
                    HourlyRate = 100.00m,
                    UserID = 1,
                    StatusID = 1  // Pending StatusID
                },
                new ClaimModel
                {
                    ClaimID = 2,
                    ClaimAmount = 150.00m,
                    SubmissionDate = DateTime.Now,
                    ClaimType = "Consultation",
                    HoursWorked = 2,
                    HourlyRate = 75.00m,
                    UserID = 1,
                    StatusID = 1  // Pending StatusID
                }
            };

                // Add the entities to the context
                context.Users.Add(user);
                context.Claims.AddRange(claims);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error seeding test data", ex);
            }
        }
    }
}