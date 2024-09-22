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
using Microsoft.AspNetCore.Http;

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
        _mockPomodoroTimerService = new Mock<PomodoroTimerService>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _mockTaskServiceRepository = new Mock<ITaskServiceRepository>();

        _controller = new PomodoroSessionController(
            _mockSessionRepository.Object,
            _mockPomodoroTimerService.Object,
            _mockUserManager.Object,
            _mockTaskServiceRepository.Object
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task CreatePomodoroSession_ValidInput_ReturnsCreated()
    {
        var sessionDto = new PomodoroSessionCreateDto
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        var user = new ApplicationUser { Id = "test-user-id" };
        var taskService = new TaskService { TaskId = sessionDto.TaskId };

        _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mockTaskServiceRepository.Setup(ts => ts.GetTaskByIdAsync(It.IsAny<Guid>())).ReturnsAsync(taskService);
        _mockSessionRepository.Setup(sr => sr.CreateSessionAsync(It.IsAny<PomodoroSession>())).Returns(Task.CompletedTask);

        var result = await _controller.CreatePomodoroSession(sessionDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<PomodoroSessionResponseDto>(createdResult.Value);
        Assert.Equal(sessionDto.TaskId, returnValue.TaskId);
    }

    [Fact]
    public async Task CreatePomodoroSession_InvalidUser_ReturnsBadRequest()
    {
        var sessionDto = new PomodoroSessionCreateDto
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

        var result = await _controller.CreatePomodoroSession(sessionDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid UserId.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreatePomodoroSession_InvalidTask_ReturnsBadRequest()
    {
        var sessionDto = new PomodoroSessionCreateDto
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        var user = new ApplicationUser { Id = "test-user-id" };

        _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mockTaskServiceRepository.Setup(ts => ts.GetTaskByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TaskService)null);

        var result = await _controller.CreatePomodoroSession(sessionDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid TaskId.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetPomodoroSessionById_ValidId_ReturnsOk()
    {
        var sessionId = Guid.NewGuid();
        var session = new PomodoroSession
        {
            SessionId = sessionId,
            UserId = "test-user-id",
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        _mockSessionRepository.Setup(sr => sr.GetSessionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(session);

        var result = await _controller.GetPomodoroSessionById(sessionId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PomodoroSessionResponseDto>(okResult.Value);
        Assert.Equal(sessionId, returnValue.SessionId);
    }

    [Fact]
    public async Task GetPomodoroSessionById_InvalidId_ReturnsNotFound()
    {
        _mockSessionRepository.Setup(sr => sr.GetSessionByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PomodoroSession)null);

        var result = await _controller.GetPomodoroSessionById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdatePomodoroSession_ValidInput_ReturnsNoContent()
    {
        var sessionId = Guid.NewGuid();
        var sessionDto = new PomodoroSessionCreateDto
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        var existingSession = new PomodoroSession
        {
            SessionId = sessionId,
            UserId = "test-user-id",
            TaskId = sessionDto.TaskId,
            StartTime = sessionDto.StartTime,
            EndTime = sessionDto.EndTime,
            BreakDuration = sessionDto.BreakDuration,
            Status = sessionDto.Status
        };

        var taskService = new TaskService { TaskId = sessionDto.TaskId };

        _mockSessionRepository.Setup(sr => sr.GetSessionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingSession);
        _mockTaskServiceRepository.Setup(ts => ts.GetTaskByIdAsync(It.IsAny<Guid>())).ReturnsAsync(taskService);
        _mockSessionRepository.Setup(sr => sr.UpdateSessionAsync(It.IsAny<PomodoroSession>())).Returns(Task.CompletedTask);

        var result = await _controller.UpdatePomodoroSession(sessionId, sessionDto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdatePomodoroSession_InvalidSession_ReturnsNotFound()
    {
        var sessionDto = new PomodoroSessionCreateDto
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        _mockSessionRepository.Setup(sr => sr.GetSessionByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PomodoroSession)null);

        var result = await _controller.UpdatePomodoroSession(Guid.NewGuid(), sessionDto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdatePomodoroSession_InvalidTask_ReturnsBadRequest()
    {
        var sessionId = Guid.NewGuid();
        var sessionDto = new PomodoroSessionCreateDto
        {
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        var existingSession = new PomodoroSession
        {
            SessionId = sessionId,
            UserId = "test-user-id",
            TaskId = sessionDto.TaskId,
            StartTime = sessionDto.StartTime,
            EndTime = sessionDto.EndTime,
            BreakDuration = sessionDto.BreakDuration,
            Status = sessionDto.Status
        };

        _mockSessionRepository.Setup(sr => sr.GetSessionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingSession);
        _mockTaskServiceRepository.Setup(ts => ts.GetTaskByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TaskService)null);

        var result = await _controller.UpdatePomodoroSession(sessionId, sessionDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid TaskId.", badRequestResult.Value);
    }

    [Fact]
    public async Task DeletePomodoroSession_ValidId_ReturnsNoContent()
    {
        var sessionId = Guid.NewGuid();
        var session = new PomodoroSession
        {
            SessionId = sessionId,
            UserId = "test-user-id",
            TaskId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(25),
            BreakDuration = 5,
            Status = "InProgress"
        };

        _mockSessionRepository.Setup(sr => sr.GetSessionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(session);
        _mockSessionRepository.Setup(sr => sr.DeleteSessionAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var result = await _controller.DeletePomodoroSession(sessionId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePomodoroSession_InvalidId_ReturnsNotFound()
    {
        _mockSessionRepository.Setup(sr => sr.GetSessionByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PomodoroSession)null);

        var result = await _controller.DeletePomodoroSession(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task StartPomodoroSession_ValidId_ReturnsNoContent()
    {
        _mockPomodoroTimerService.Setup(pts => pts.StartSessionAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var result = await _controller.StartPomodoroSession(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task StartPomodoroSession_InvalidId_ReturnsNotFound()
    {
        _mockPomodoroTimerService.Setup(pts => pts.StartSessionAsync(It.IsAny<Guid>())).Throws<ArgumentException>();

        var result = await _controller.StartPomodoroSession(Guid.NewGuid());

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Session not found.", notFoundResult.Value);
    }

    [Fact]
    public void PausePomodoroSession_ValidId_ReturnsNoContent()
    {
        _mockPomodoroTimerService.Setup(pts => pts.PauseSession());

        var result = _controller.PausePomodoroSession(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void PausePomodoroSession_InvalidId_ReturnsNotFound()
    {
        _mockPomodoroTimerService.Setup(pts => pts.PauseSession()).Throws<ArgumentException>();

        var result = _controller.PausePomodoroSession(Guid.NewGuid());

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Session not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task ResumePomodoroSession_ValidId_ReturnsNoContent()
    {
        _mockPomodoroTimerService.Setup(pts => pts.ResumeSessionAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var result = await _controller.ResumePomodoroSession(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ResumePomodoroSession_InvalidId_ReturnsNotFound()
    {
        _mockPomodoroTimerService.Setup(pts => pts.ResumeSessionAsync(It.IsAny<Guid>())).Throws<ArgumentException>();

        var result = await _controller.ResumePomodoroSession(Guid.NewGuid());

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Session not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task StopPomodoroSession_ValidId_ReturnsNoContent()
    {
        _mockPomodoroTimerService.Setup(pts => pts.StopSessionAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var result = await _controller.StopPomodoroSession(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task StopPomodoroSession_InvalidId_ReturnsNotFound()
    {
        _mockPomodoroTimerService.Setup(pts => pts.StopSessionAsync(It.IsAny<Guid>())).Throws<ArgumentException>();

        var result = await _controller.StopPomodoroSession(Guid.NewGuid());

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Session not found.", notFoundResult.Value);
    }
}