using ToBee.API.Data;
using ToBee.API.Models;

namespace ToBee.API.Repositories.ApplicationUserRepository
{
	public class ApplicationUserRepository : IApplicationUserRepository
	{
		private readonly AppDbContext _context;

		public ApplicationUserRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<ApplicationUser> GetUserByIdAsync(string userId)
		{
			return await _context.Users.FindAsync(userId);
		}

		public async Task UpdateUserAsync(ApplicationUser user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}
	}
}
