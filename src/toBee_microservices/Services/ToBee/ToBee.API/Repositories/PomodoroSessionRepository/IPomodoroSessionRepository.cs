using ToBee.API.Models;

namespace ToBee.API.Repositories.PomodoroSessionRepository
{
	public interface IPomodoroSessionRepository
	{
		Task CreateSessionAsync(PomodoroSession session);
		Task<PomodoroSession> GetSessionByIdAsync(Guid id);
		Task UpdateSessionAsync(PomodoroSession session);
		Task DeleteSessionAsync(Guid id);
	}

}
