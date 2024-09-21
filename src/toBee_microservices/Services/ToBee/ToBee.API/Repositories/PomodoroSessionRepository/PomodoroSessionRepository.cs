using ToBee.API.Data;
using ToBee.API.Models;

namespace ToBee.API.Repositories.PomodoroSessionRepository
{
	public class PomodoroSessionRepository : IPomodoroSessionRepository
	{
		private readonly AppDbContext _context;

		public PomodoroSessionRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task CreateSessionAsync(PomodoroSession session)
		{
			_context.PomodoroSessions.Add(session);
			await _context.SaveChangesAsync();
		}

		public async Task<PomodoroSession> GetSessionByIdAsync(Guid id)
		{
			return await _context.PomodoroSessions.FindAsync(id);
		}

		public async Task UpdateSessionAsync(PomodoroSession session)
		{
			_context.PomodoroSessions.Update(session);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteSessionAsync(Guid id)
		{
			var session = await _context.PomodoroSessions.FindAsync(id);
			if (session != null)
			{
				_context.PomodoroSessions.Remove(session);
				await _context.SaveChangesAsync();
			}
		}
	}

}
