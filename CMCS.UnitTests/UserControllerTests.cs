/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

using CMCS.Controllers;
using CMCS.Data;
using CMCS.Models;
using CMCS.UnitTests.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace CMCS.UnitTests
{
    [TestClass]
    public class UserControllerTests
    {
        // Declare the DbContextOptions and Mock ILogger for use in the tests

        private DbContextOptions<CMCSDbContext> options;
        private Mock<ILogger<UserController>> mockLogger;

        // The Setup method is decorated with the TestInitialize attribute to run before each test. This method creates a new in-memory database and a mock logger for use in the tests.
        [TestInitialize]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<CMCSDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Suppress transaction warning
                .Options;

            mockLogger = new Mock<ILogger<UserController>>();
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the User action returns the user details when the user is authenticated.
        [TestMethod]
        public async Task User_AuthenticatedUser_ReturnsUserDetails()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data using TestDataSeeder
                TestDataSeeder.SeedTestData(context);
            }

            using (var context = new CMCSDbContext(options))
            {
                var controller = new UserController(context, mockLogger.Object);

                // Mock HttpContext and session
                var mockHttpContext = new DefaultHttpContext();
                var session = new Mock<ISession>();
                var sessionStorage = new Dictionary<string, byte[]>();

                session.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                       .Callback<string, byte[]>((key, value) => sessionStorage[key] = value);
                session.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                       .Returns((string key, out byte[] value) =>
                       {
                           return sessionStorage.TryGetValue(key, out value);
                       });
                mockHttpContext.Session = session.Object;
                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext
                };

                // Set the UserID in the session
                controller.HttpContext.Session.SetInt32("UserID", 1);

                // Act
                var result = await controller.User() as ViewResult;

                // Assert
                Assert.IsNotNull(result, "Result is null");
                Assert.IsInstanceOfType(result.Model, typeof(UserModel), "Model is not of type UserModel");

                var userModel = result.Model as UserModel;
                Assert.AreEqual(1, userModel.UserID, "UserID does not match");
                Assert.AreEqual("JohnDoe", userModel.UserName, "UserName does not match");
                Assert.AreEqual("John", userModel.FirstName, "FirstName does not match");
                Assert.AreEqual("Doe", userModel.LastName, "LastName does not match");
                Assert.AreEqual("john.doe@example.com", userModel.UserEmail, "UserEmail does not match");
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the add lecturer action adds a lecturer successfully when the data is valid.
        [TestMethod]
        public async Task AddLecturer_ValidData_AddsLecturerSuccessfully()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data using TestDataSeeder to avoid conflicts
                TestDataSeeder.SeedTestData(context);
            }
            // Create a new context to verify the data was added to the database
            using (var context = new CMCSDbContext(options))
            {
                var controller = new UserController(context, mockLogger.Object);

                // Create a new UserModel to simulate adding a lecturer
                var newLecturer = new UserModel
                {
                    UserName = "janedoe",
                    FirstName = "Jane",
                    LastName = "Doe",
                    UserEmail = "jane.doe@example.com",
                    PhoneNumber = "0987654321",
                    Address = "456 Another St",
                    BankAccountNumber = "9876543210",
                    BankName = "Bank B",
                    BranchCode = "654321",
                    UserPassword = "Password123"
                };

                // Act: Call the AddLecturer action with the new lecturer data
                var result = await controller.AddLecturer(newLecturer) as JsonResult;

                // Assert: Check that the result is not null and the action was successful
                Assert.IsNotNull(result, "Result is null");
                var successProperty = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                Assert.IsTrue((bool)successProperty, "The success property was not true.");

                var userIdProperty = result.Value.GetType().GetProperty("id")?.GetValue(result.Value, null);
                Assert.IsNotNull(userIdProperty, "UserID is null");
                Assert.AreNotEqual(0, (int)userIdProperty, "UserID should not be 0");

                // Verify the lecturer was added to the database
                var addedLecturer = await context.Users.FirstOrDefaultAsync(u => u.UserName == "janedoe");
                Assert.IsNotNull(addedLecturer, "Lecturer was not added to the database");
                Assert.AreEqual("Jane", addedLecturer.FirstName, "FirstName does not match");
                Assert.AreEqual("Doe", addedLecturer.LastName, "LastName does not match");
                Assert.AreEqual("jane.doe@example.com", addedLecturer.UserEmail, "UserEmail does not match");
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the AddLecturer action returns an error when the data is a duplicate.
        [TestMethod]
        public async Task AddLecturer_DuplicateData_ReturnsError()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data using TestDataSeeder, which already contains a user
                TestDataSeeder.SeedTestData(context);
            }

            using (var context = new CMCSDbContext(options))
            {
                var controller = new UserController(context, mockLogger.Object);

                // Create a new UserModel with duplicate email (same as JohnDoe from TestDataSeeder)
                var duplicateLecturer = new UserModel
                {
                    UserName = "newuser",
                    FirstName = "John",
                    LastName = "Doe",
                    UserEmail = "john.doe@example.com", // Duplicate email
                    PhoneNumber = "0987654321",
                    Address = "456 Another St",
                    BankAccountNumber = "9876543210",
                    BankName = "Bank B",
                    BranchCode = "654321",
                    UserPassword = "Password123"
                };

                // Act: Call the AddLecturer action with the duplicate email
                var result = await controller.AddLecturer(duplicateLecturer) as JsonResult;

                // Assert: Check that the result is not null and the action failed
                Assert.IsNotNull(result, "Result is null");
                var successProperty = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                Assert.IsFalse((bool)successProperty, "The success property should be false.");

                var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
                Assert.AreEqual("A user with this email already exists.", message, "The error message was not correct.");
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the AddLecturer action returns an error when the data is invalid.
        [TestMethod]
        public async Task AddLecturer_InvalidData_ReturnsError()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data using TestDataSeeder
                TestDataSeeder.SeedTestData(context);
            }

            using (var context = new CMCSDbContext(options))
            {
                var controller = new UserController(context, mockLogger.Object);

                // Manually invalidate ModelState for missing required fields
                controller.ModelState.AddModelError("UserName", "The UserName field is required.");
                controller.ModelState.AddModelError("UserEmail", "The UserEmail field is required.");

                // Create a new UserModel with missing required fields
                var invalidLecturer = new UserModel
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    PhoneNumber = "0987654321",
                    Address = "456 Another St",
                    BankAccountNumber = "9876543210",
                    BankName = "Bank B",
                    BranchCode = "654321",
                    UserPassword = "Password123"
                };

                // Act: Call the AddLecturer action with invalid data
                var result = await controller.AddLecturer(invalidLecturer) as JsonResult;

                // Assert: Check that the result is not null and the action failed
                Assert.IsNotNull(result, "Result is null");
                var successProperty = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                Assert.IsFalse((bool)successProperty, "The success property should be false.");

                // Check if there are error messages
                var errors = result.Value.GetType().GetProperty("errors")?.GetValue(result.Value, null) as IEnumerable<string>;
                Assert.IsNotNull(errors, "Errors property is null.");

                // Verify specific error messages for missing fields
                Assert.IsTrue(errors.Contains("The UserName field is required."), "Missing error for UserName.");
                Assert.IsTrue(errors.Contains("The UserEmail field is required."), "Missing error for UserEmail.");
            }
        }
    }
}