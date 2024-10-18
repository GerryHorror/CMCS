using CMCS.Controllers;
using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using CMCS.UnitTests.Helper;

namespace CMCS.UnitTests
{
    [TestClass]
    public class ApprovalControllerTests
    {
        // Declare the DbContextOptions and Mock ILogger for use in the tests

        private DbContextOptions<CMCSDbContext> options;
        private Mock<ILogger<ApprovalController>> mockLogger;

        // The Setup method is decorated with the TestInitialize attribute to run before each test. This method creates a new in-memory database and a mock logger for use in the tests.
        [TestInitialize]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<CMCSDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            mockLogger = new Mock<ILogger<ApprovalController>>();
        }

        // This test verifies that the Verify action returns a view with a list of claims when the claims are valid.
        [TestMethod]
        public async Task Verify_ValidClaims_ReturnsViewWithClaims()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Use the test data seeder
                TestDataSeeder.SeedTestData(context);

                // Act: Call the Verify action
                var controller = new ApprovalController(context, mockLogger.Object);
                var result = await controller.Verify() as ViewResult;

                // Assert: Verify the result
                Assert.IsNotNull(result);
                // Ensure the model is a List<ClaimModel>
                Assert.IsInstanceOfType(result.Model, typeof(List<ClaimModel>));
                var model = result.Model as List<ClaimModel>;
                // Ensure the model contains 2 claims
                Assert.AreEqual(2, model.Count);
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the UpdateStatus action approves a claim when the claim and status are valid.
        [TestMethod]
        public async Task UpdateStatus_ValidClaim_ApprovesClaim()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data
                TestDataSeeder.SeedTestData(context);

                // Arrange
                var controller = new ApprovalController(context, mockLogger.Object);
                var updateModel = new UpdateStatusModel
                {
                    // Valid ClaimId
                    ClaimId = 1,
                    // Valid status
                    Status = "Approved"
                };

                // Act: Call the UpdateStatus method to approve the claim
                var result = await controller.UpdateStatus(updateModel) as JsonResult;

                // Assert: Verify that the result is a JsonResult and contains a value
                Assert.IsNotNull(result, "Result is null.");
                Assert.IsNotNull(result.Value, "JsonResult does not contain a value.");

                // Use reflection to access properties of the anonymous object
                var success = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);

                // Assert that the success property is true
                Assert.IsNotNull(success, "The success property was not found.");
                Assert.IsTrue((bool)success, "The success property was not true.");

                // Assert that the message property contains the correct value
                Assert.IsNotNull(message, "The message property was not found.");
                Assert.AreEqual("claim 1 has been approved.", message.ToString().ToLower());

                // Verify that the claim's status is updated to "Approved"
                var updatedClaim = context.Claims.Find(1);
                var approvedStatus = context.ClaimStatuses.FirstOrDefault(s => s.StatusName == "Approved");
                Assert.AreEqual(approvedStatus.StatusID, updatedClaim.StatusID);
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the UpdateStatus action rejects a claim when the claim and status are valid.
        [TestMethod]
        public async Task UpdateStatus_ValidClaim_RejectsClaim()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data
                TestDataSeeder.SeedTestData(context);

                // Arrange
                var controller = new ApprovalController(context, mockLogger.Object);
                var updateModel = new UpdateStatusModel
                {
                    // Valid ClaimId
                    ClaimId = 1,
                    // Valid status
                    Status = "Rejected"
                };

                // Act: Call the UpdateStatus method to reject the claim
                var result = await controller.UpdateStatus(updateModel) as JsonResult;

                // Assert: Verify that the result is a JsonResult and contains a value
                Assert.IsNotNull(result, "Result is null.");
                Assert.IsNotNull(result.Value, "JsonResult does not contain a value.");

                // Use reflection to access properties of the anonymous object
                var success = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);

                // Assert that the success property is true
                Assert.IsNotNull(success, "The success property was not found.");
                Assert.IsTrue((bool)success, "The success property was not true.");

                // Assert that the message property contains the correct value
                Assert.IsNotNull(message, "The message property was not found.");
                Assert.AreEqual("claim 1 has been rejected.", message.ToString().ToLower());

                // Verify that the claim's status is updated to "Rejected"
                var updatedClaim = context.Claims.Find(1);
                var rejectedStatus = context.ClaimStatuses.FirstOrDefault(s => s.StatusName == "Rejected");
                Assert.AreEqual(rejectedStatus.StatusID, updatedClaim.StatusID);
            }
        }
    }
}