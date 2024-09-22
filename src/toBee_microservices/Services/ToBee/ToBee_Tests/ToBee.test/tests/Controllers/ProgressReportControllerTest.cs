using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ToBee.API.Controllers;
using ToBee.API.Models;
using ToBee.API.Services.ProgressReportService;
using Xunit;

namespace ToBee.API.Tests.Controllers
{
    public class ProgressReportControllerTest
    {
        private readonly Mock<IProgressReportService> _progressReportServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly ProgressReportController _controller;

        public ProgressReportControllerTest()
        {
            _progressReportServiceMock = new Mock<IProgressReportService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _controller = new ProgressReportController(_progressReportServiceMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task GetProgressReport_ReturnsOkResult_WithProgressReportDto()
        {
            // Arrange
            var userId = "test-user-id";
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var progressReport = new ProgressReport
            {
                ReportId = Guid.NewGuid(),
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                TotalFocusTime = 10.5,
                TotalTasksCompleted = 5
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);
            _progressReportServiceMock.Setup(x => x.GenerateProgressReportAsync(userId, startDate, endDate)).ReturnsAsync(progressReport);

            // Act
            var result = await _controller.GetProgressReport(startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ProgressReportDto>(okResult.Value);
            Assert.Equal(progressReport.ReportId, returnValue.ReportId);
            Assert.Equal(progressReport.UserId, returnValue.UserId);
            Assert.Equal(progressReport.StartDate, returnValue.StartDate);
            Assert.Equal(progressReport.EndDate, returnValue.EndDate);
            Assert.Equal(progressReport.TotalFocusTime, returnValue.TotalFocusTime);
            Assert.Equal(progressReport.TotalTasksCompleted, returnValue.TotalTasksCompleted);
        }

        [Fact]
        public async Task GetProgressReport_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal());

            // Act
            var result = await _controller.GetProgressReport(DateTime.Now.AddDays(-7), DateTime.Now);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }
    }
}