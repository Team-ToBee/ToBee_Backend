using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ToBee.API.Controllers;
using ToBee.API.Models;
using ToBee.API.Models.DTOs;
using ToBee.API.Repositories.PomodoroSessionRepository;
using ToBee.API.Repositories.TaskServiceRepository;
using ToBee.API.Services;
using Xunit;

namespace ToBee.API.Tests.Controllers
{
	public class PomodoroSessionControllerTest
	{
		private readonly Mock<IPomodoroSessionRepository> _mockSessionRepository;
		private readonly Mock<PomodoroTimerService> _mockPomodoroTimerService;
		private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
		private readonly Mock<ITaskServiceRepository> _mockTaskServiceRepository;
		private readonly PomodoroSessionController _controller;

		public PomodoroSessionControllerTest()
		{
			_mockSessionRepository = new Mock<IPomodoroSessionRepository>();
			_mockPomodoroTimerService = new Mock<PomodoroTimerService>(_mockSessionRepository.Object);
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
			_mockTaskServiceRepository = new Mock<ITaskServiceRepository>();

			_controller = new PomodoroSessionController(
				_mockSessionRepository.Object,
				_mockPomodoroTimerService.Object,
				_mockUserManager.Object,
				_mockTaskServiceRepository.Object
			);
		}

		[Fact]
		public async Task CreatePomodoroSession_ReturnsCreatedResult_WhenSessionIsValid()
		{
			// Arrange
			var sessionDto = new PomodoroSessionCreateDto
			{
				TaskId = Guid.NewGuid(),
				StartTime = DateTime.UtcNow
			};

			var session = new PomodoroSession
			{
				SessionId = Guid.NewGuid(),
				TaskId = sessionDto.TaskId,
				StartTime = sessionDto.StartTime
			};

			_mockSessionRepository.Setup(repo => repo.CreateSessionAsync(It.IsAny<PomodoroSession>()))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.CreatePomodoroSession(sessionDto);

			// Assert
			var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			Assert.IsType<PomodoroSessionResponseDto>(createdResult.Value);
		}

		[Fact]
		public async Task GetPomodoroSessionById_ReturnsOkResult_WhenSessionExists()
		{
			// Arrange
			var sessionId = Guid.NewGuid();
			var session = new PomodoroSession
			{
				SessionId = sessionId,
				UserId = "test-user-id",
				TaskId = Guid.NewGuid(),
				StartTime = DateTime.UtcNow
			};

			_mockSessionRepository.Setup(repo => repo.GetSessionByIdAsync(sessionId))
				.ReturnsAsync(session);

			// Act
			var result = await _controller.GetPomodoroSessionById(sessionId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			Assert.IsType<PomodoroSessionResponseDto>(okResult.Value);
		}

		[Fact]
		public async Task GetPomodoroSessionById_ReturnsNotFound_WhenSessionDoesNotExist()
		{
			// Arrange
			var sessionId = Guid.NewGuid();

			_mockSessionRepository.Setup(repo => repo.GetSessionByIdAsync(sessionId))
				.ReturnsAsync((PomodoroSession)null);

			// Act
			var result = await _controller.GetPomodoroSessionById(sessionId);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

	}
}
