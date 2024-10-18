/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

using CMCS.Controllers;
using CMCS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

[TestClass]
public class HomeControllerTests
{
    // Declare the DbContextOptions and Mock ILogger for use in the tests

    private DbContextOptions<CMCSDbContext> options;
    private Mock<ILogger<HomeController>> mockLogger;

    // The Setup method is decorated with the TestInitialize attribute to run before each test. This method creates a new in-memory database and a mock logger for use in the tests.
    [TestInitialize]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<CMCSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        mockLogger = new Mock<ILogger<HomeController>>();
    }

    // <---------------------------------------------------------------------------------------------------------------------------->

    // This test verifies that the Index action redirects to the Login page when the user is unauthenticated.
    [TestMethod]
    public void Index_UnauthenticatedUser_RedirectsToLogin()
    {
        using (var context = new CMCSDbContext(options))
        {
            var controller = new HomeController(mockLogger.Object, context);

            // Mock HttpContext and session
            var mockHttpContext = new DefaultHttpContext();
            var session = new Mock<ISession>();

            // No UserRole set in the session (unauthenticated)
            mockHttpContext.Session = session.Object;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            };

            // Act
            var result = controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result, "Result is null");
            Assert.AreEqual("Login", result.ActionName, "Expected to be redirected to Login page");
        }
    }

    // <---------------------------------------------------------------------------------------------------------------------------->

    // This test verifies that the Index action returns a view when the user is authenticated.
    [TestMethod]
    public void Index_AuthenticatedUser_ReturnsView()
    {
        using (var context = new CMCSDbContext(options))
        {
            var controller = new HomeController(mockLogger.Object, context);

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

            // Simulate UserRole being set in the session (authenticated)
            var userRoleBytes = Encoding.UTF8.GetBytes("Admin");
            sessionStorage["UserRole"] = userRoleBytes;
            mockHttpContext.Session = session.Object;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            };

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result is null");
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Expected a ViewResult");
        }
    }
}