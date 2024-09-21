using ToBee.API.Repositories.PomodoroSessionRepository;
using ToBee.API.Models;

namespace ToBee.API.Services
{
	public class PomodoroTimerService
	{
		private readonly IPomodoroSessionRepository _sessionRepository;
		private CancellationTokenSource _cancellationTokenSource;

		public PomodoroTimerService(IPomodoroSessionRepository sessionRepository)
		{
			_sessionRepository = sessionRepository;
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public async Task StartSessionAsync(Guid sessionId)
		{
			var session = await _sessionRepository.GetSessionByIdAsync(sessionId);
			if (session == null) throw new ArgumentException("Session not found");

			session.StartTime = DateTime.UtcNow;
			session.Status = "In Progress";
			await _sessionRepository.UpdateSessionAsync(session);

			// Start a timer for the work interval
			await Task.Delay(TimeSpan.FromMinutes(25), _cancellationTokenSource.Token);
			await StartBreakAsync(sessionId);
		}

		public void PauseSession()
		{
			_cancellationTokenSource.Cancel();
		}

		public async Task ResumeSessionAsync(Guid sessionId)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			var session = await _sessionRepository.GetSessionByIdAsync(sessionId);
			if (session == null) throw new ArgumentException("Session not found");

			if (session.Status == "In Progress")
			{
				// Resume work interval
				await Task.Delay(TimeSpan.FromMinutes(25), _cancellationTokenSource.Token);
				await StartBreakAsync(sessionId);
			}
			else if (session.Status == "Break")
			{
				// Resume break interval
				await Task.Delay(TimeSpan.FromMinutes(5), _cancellationTokenSource.Token);
				await EndSessionAsync(sessionId);
			}
		}

		public async Task StopSessionAsync(Guid sessionId)
		{
			_cancellationTokenSource.Cancel();
			var session = await _sessionRepository.GetSessionByIdAsync(sessionId);
			if (session == null) throw new ArgumentException("Session not found");

			session.EndTime = DateTime.UtcNow;
			session.Status = "Stopped";
			await _sessionRepository.UpdateSessionAsync(session);
		}

		private async Task StartBreakAsync(Guid sessionId)
		{
			var session = await _sessionRepository.GetSessionByIdAsync(sessionId);
			if (session == null) throw new ArgumentException("Session not found");

			session.Status = "Break";
			await _sessionRepository.UpdateSessionAsync(session);

			// Start a timer for the break interval
			await Task.Delay(TimeSpan.FromMinutes(5), _cancellationTokenSource.Token);
			await EndSessionAsync(sessionId);
		}

		private async Task EndSessionAsync(Guid sessionId)
		{
			var session = await _sessionRepository.GetSessionByIdAsync(sessionId);
			if (session == null) throw new ArgumentException("Session not found");

			session.EndTime = DateTime.UtcNow;
			session.Status = "Completed";
			await _sessionRepository.UpdateSessionAsync(session);
		}
	}
}
