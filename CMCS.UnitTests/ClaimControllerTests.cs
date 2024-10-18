using Microsoft.AspNetCore.Mvc;
using CMCS.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;
using CMCS.UnitTests.Helper;
using Microsoft.AspNetCore.Http;

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
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            mockLogger = new Mock<ILogger<ClaimController>>();
        }
    }
}