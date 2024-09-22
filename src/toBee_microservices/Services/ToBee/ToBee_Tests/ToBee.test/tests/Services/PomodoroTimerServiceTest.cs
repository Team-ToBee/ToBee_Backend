using Moq;
using ToBee.API.Repositories.PomodoroSessionRepository;
using ToBee.API.Models;
using ToBee.API.Services;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Threading;

public class PomodoroTimerServiceTest
{
    private readonly Mock<IPomodoroSessionRepository> _mockSessionRepository;
    private readonly PomodoroTimerService _pomodoroTimerService;

    public PomodoroTimerServiceTest()
    {
        _mockSessionRepository = new Mock<IPomodoroSessionRepository>();
        _pomodoroTimerService = new PomodoroTimerService(_mockSessionRepository.Object);
    }

    [Fact]
    public async Task StartSessionAsync_ShouldStartSession()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new PomodoroSession { SessionId = sessionId, Status = "Pending" };
        _mockSessionRepository.Setup(repo => repo.GetSessionByIdAsync(sessionId)).ReturnsAsync(session);

        // Act
        await _pomodoroTimerService.StartSessionAsync(sessionId);

        // Assert
        _mockSessionRepository.Verify(repo => repo.UpdateSessionAsync(It.Is<PomodoroSession>(s => s.Status == "In Progress" && s.StartTime != null)), Times.Once);
    }

   

    [Fact]
    public async Task ResumeSessionAsync_ShouldResumeWorkInterval()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new PomodoroSession { SessionId = sessionId, Status = "In Progress" };
        _mockSessionRepository.Setup(repo => repo.GetSessionByIdAsync(sessionId)).ReturnsAsync(session);

        // Act
        await _pomodoroTimerService.ResumeSessionAsync(sessionId);

        // Assert
        _mockSessionRepository.Verify(repo => repo.UpdateSessionAsync(It.Is<PomodoroSession>(s => s.Status == "Break")), Times.Once);
    }

    [Fact]
    public async Task StopSessionAsync_ShouldStopSession()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new PomodoroSession { SessionId = sessionId, Status = "In Progress" };
        _mockSessionRepository.Setup(repo => repo.GetSessionByIdAsync(sessionId)).ReturnsAsync(session);

        // Act
        await _pomodoroTimerService.StopSessionAsync(sessionId);

        // Assert
        _mockSessionRepository.Verify(repo => repo.UpdateSessionAsync(It.Is<PomodoroSession>(s => s.Status == "Stopped" && s.EndTime != null)), Times.Once);
    }
}