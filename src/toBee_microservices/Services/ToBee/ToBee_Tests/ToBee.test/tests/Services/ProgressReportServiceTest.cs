using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using ToBee.API.Data;
using ToBee.API.Models;
using ToBee.API.Repositories.RewardRepository;
using ToBee.API.Services.ProgressReportService;
using Xunit;

namespace ToBee.API.Tests.Services
{
    public class ProgressReportServiceTest
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<IRewardRepository> _mockRewardRepository;
        private readonly ProgressReportService _service;

        public ProgressReportServiceTest()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockRewardRepository = new Mock<IRewardRepository>();
            _service = new ProgressReportService(_mockContext.Object, _mockRewardRepository.Object);
        }

        [Fact]
        public async Task GenerateProgressReportAsync_ShouldReturnCorrectReport()
        {
            // Arrange
            var userId = "test-user";
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 31);

            var pomodoroSessions = new List<PomodoroSession>
            {
                new PomodoroSession { UserId = userId, StartTime = new DateTime(2023, 1, 10, 9, 0, 0), EndTime = new DateTime(2023, 1, 10, 10, 0, 0), BreakDuration = 10 },
                new PomodoroSession { UserId = userId, StartTime = new DateTime(2023, 1, 15, 14, 0, 0), EndTime = new DateTime(2023, 1, 15, 15, 0, 0), BreakDuration = 5 }
            }.AsQueryable();

            var taskServices = new List<TaskService>
            {
                new TaskService { UserId = userId, Status = Models.TaskStatus.Done, CompletedAt = new DateTime(2023, 1, 20) },
                new TaskService { UserId = userId, Status = Models.TaskStatus.Done, CompletedAt = new DateTime(2023, 1, 25) }
            }.AsQueryable();

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = userId, UserName = "Test User" }
            }.AsQueryable();

            var mockPomodoroSessionsSet = new Mock<DbSet<PomodoroSession>>();
            mockPomodoroSessionsSet.As<IQueryable<PomodoroSession>>().Setup(m => m.Provider).Returns(pomodoroSessions.Provider);
            mockPomodoroSessionsSet.As<IQueryable<PomodoroSession>>().Setup(m => m.Expression).Returns(pomodoroSessions.Expression);
            mockPomodoroSessionsSet.As<IQueryable<PomodoroSession>>().Setup(m => m.ElementType).Returns(pomodoroSessions.ElementType);
            mockPomodoroSessionsSet.As<IQueryable<PomodoroSession>>().Setup(m => m.GetEnumerator()).Returns(pomodoroSessions.GetEnumerator());

            var mockTaskServicesSet = new Mock<DbSet<TaskService>>();
            mockTaskServicesSet.As<IQueryable<TaskService>>().Setup(m => m.Provider).Returns(taskServices.Provider);
            mockTaskServicesSet.As<IQueryable<TaskService>>().Setup(m => m.Expression).Returns(taskServices.Expression);
            mockTaskServicesSet.As<IQueryable<TaskService>>().Setup(m => m.ElementType).Returns(taskServices.ElementType);
            mockTaskServicesSet.As<IQueryable<TaskService>>().Setup(m => m.GetEnumerator()).Returns(taskServices.GetEnumerator());

            var mockUsersSet = new Mock<DbSet<ApplicationUser>>();
            mockUsersSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUsersSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUsersSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            

            _mockContext.Setup(c => c.PomodoroSessions).Returns(mockPomodoroSessionsSet.Object);
            _mockContext.Setup(c => c.TaskServices).Returns(mockTaskServicesSet.Object);
            _mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);

            // Act
            var result = await _service.GenerateProgressReportAsync(userId, startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(startDate, result.StartDate);
            Assert.Equal(endDate, result.EndDate);
            Assert.Equal(105, result.TotalFocusTime); // 60 - 10 + 60 - 5
            Assert.Equal(2, result.TotalTasksCompleted);
            Assert.Equal("Test User", result.User.UserName);
        }
    }
}