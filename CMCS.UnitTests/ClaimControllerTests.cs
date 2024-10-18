using Microsoft.AspNetCore.Mvc;
using CMCS.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;
using CMCS.Models;
using CMCS.UnitTests.Helper;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CMCS.UnitTests
{
    [TestClass]
    public class ClaimControllerTests
    {
        // Declare the DbContextOptions and Mock ILogger for use in the tests

        private DbContextOptions<CMCSDbContext> options;
        private Mock<ILogger<ClaimController>> mockLogger;

        // The Setup method is decorated with the TestInitialize attribute to run before each test. This method creates a new in-memory database and a mock logger for use in the tests.
        [TestInitialize]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<CMCSDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Suppress transaction warning
                .Options;

            mockLogger = new Mock<ILogger<ClaimController>>();
        }

        // This test verifies that the Claim action returns a view with a list of claims when the user is authenticated.
        [TestMethod]
        public async Task Claim_AuthenticatedUser_ReturnsViewWithClaims()
        {
            // Arrange
            using (var context = new CMCSDbContext(options))
            {
                TestDataSeeder.SeedTestData(context);
            }

            using (var context = new CMCSDbContext(options))
            {
                var controller = new ClaimController(context, mockLogger.Object);
                // Mock HttpContext and session for the controller
                var httpContext = new DefaultHttpContext();
                var session = new Mock<ISession>();
                var sessionStorage = new Dictionary<string, byte[]>();
                // Set up session handling for the controller
                session.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                       .Callback<string, byte[]>((key, value) => sessionStorage[key] = value);

                session.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                       .Returns((string key, out byte[] value) =>
                       {
                           return sessionStorage.TryGetValue(key, out value);
                       });
                // Set the session for the controller
                httpContext.Session = session.Object;
                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                };

                // Set the UserID in the session
                controller.HttpContext.Session.SetInt32("UserID", 1);

                // Act
                var result = await controller.Claim();

                // Assert
                Assert.IsNotNull(result, "Result is null");
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult, "Result is not a ViewResult");
                Assert.IsInstanceOfType(viewResult.Model, typeof(ClaimViewModel), "Model is not of type ClaimViewModel");
                var model = viewResult.Model as ClaimViewModel;
                Assert.IsNotNull(model.Claims, "Claims list is null");
                Assert.AreEqual(2, model.Claims.Count, "Unexpected number of claims");
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the Submit action returns a success message when a valid claim is submitted.
        [TestMethod]
        public async Task Submit_ValidClaim_ReturnsSuccess()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data
                TestDataSeeder.SeedTestData(context);

                // Arrange
                var controller = new ClaimController(context, mockLogger.Object);

                // Mock HttpContext and session
                var mockHttpContext = new DefaultHttpContext();
                var session = new Mock<ISession>();
                var sessionStorage = new Dictionary<string, byte[]>();

                // Set up session handling
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

                // Create a valid ClaimModel and WorkEntries
                var claimModel = new ClaimModel
                {
                    ClaimType = "Service",
                    HourlyRate = 100.00m,
                    SubmissionDate = DateTime.Now,
                    HoursWorked = 5,
                    ClaimAmount = 500.00m,
                    UserID = 1, // Use the seeded UserID
                    StatusID = 1 // Pending status
                };

                var workEntries = new List<WorkEntry>
            {
                new WorkEntry { WorkDate = DateTime.Now.AddDays(-1), HoursWorked = 5 }
            };

                // No file (supportingDocument) for this test
                IFormFile supportingDocument = null;

                // Act
                var result = await controller.Submit(claimModel, workEntries, supportingDocument) as JsonResult;

                // Assert
                Assert.IsNotNull(result, "Result is null");
                var successProperty = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                Assert.IsTrue((bool)successProperty, "The success property was not true.");

                var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
                Assert.AreEqual("Claim submitted successfully", message, "The message was not correct.");
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the Submit action returns an error message when the file size exceeds the limit.
        [TestMethod]
        public async Task Submit_FileSizeExceedsLimit_ReturnsError()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data
                TestDataSeeder.SeedTestData(context);

                // Arrange
                var controller = new ClaimController(context, mockLogger.Object);

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

                // Create a valid ClaimModel and WorkEntries
                var claimModel = new ClaimModel
                {
                    ClaimType = "Service",
                    HourlyRate = 100.00m,
                    SubmissionDate = DateTime.Now,
                    HoursWorked = 5,
                    ClaimAmount = 500.00m,
                    UserID = 1, // Use the seeded UserID
                    StatusID = 1 // Pending status
                };

                var workEntries = new List<WorkEntry>
        {
            new WorkEntry { WorkDate = DateTime.Now.AddDays(-1), HoursWorked = 5 }
        };

                // Create a mock file that exceeds the size limit (5MB)
                var largeFile = new Mock<IFormFile>();
                largeFile.Setup(f => f.Length).Returns(6 * 1024 * 1024); // 6MB file
                largeFile.Setup(f => f.FileName).Returns("largefile.pdf");
                largeFile.Setup(f => f.ContentType).Returns("application/pdf");

                // Act
                var result = await controller.Submit(claimModel, workEntries, largeFile.Object) as JsonResult;

                // Assert
                Assert.IsNotNull(result, "Result is null");
                var successProperty = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                Assert.IsFalse((bool)successProperty, "The success property should be false.");

                var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
                Assert.AreEqual("File size exceeds 5MB limit", message, "The error message was not correct.");
            }
        }

        // <---------------------------------------------------------------------------------------------------------------------------->

        // This test verifies that the Submit action returns an error message when the file type is not supported.
        [TestMethod]
        public async Task Submit_UnsupportedFileType_ReturnsError()
        {
            using (var context = new CMCSDbContext(options))
            {
                // Seed the test data
                TestDataSeeder.SeedTestData(context);

                // Arrange
                var controller = new ClaimController(context, mockLogger.Object);

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

                // Create a valid ClaimModel and WorkEntries
                var claimModel = new ClaimModel
                {
                    ClaimType = "Service",
                    HourlyRate = 100.00m,
                    SubmissionDate = DateTime.Now,
                    HoursWorked = 5,
                    ClaimAmount = 500.00m,
                    UserID = 1, // Use the seeded UserID
                    StatusID = 1 // Pending status
                };

                var workEntries = new List<WorkEntry>
        {
            new WorkEntry { WorkDate = DateTime.Now.AddDays(-1), HoursWorked = 5 }
        };

                // Create a mock file with an unsupported file extension
                var unsupportedFile = new Mock<IFormFile>();
                unsupportedFile.Setup(f => f.Length).Returns(1024); // 1KB file
                unsupportedFile.Setup(f => f.FileName).Returns("file.txt");
                unsupportedFile.Setup(f => f.ContentType).Returns("text/plain");

                // Act
                var result = await controller.Submit(claimModel, workEntries, unsupportedFile.Object) as JsonResult;

                // Assert
                Assert.IsNotNull(result, "Result is null");
                var successProperty = result.Value.GetType().GetProperty("success")?.GetValue(result.Value, null);
                Assert.IsFalse((bool)successProperty, "The success property should be false.");

                var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
                Assert.AreEqual("Only .pdf, .docx, and .xlsx files are allowed", message, "The error message was not correct.");
            }
        }
    }
}