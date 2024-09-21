using ToBee.API.Models;

namespace ToBee.API.Repositories.ApplicationUserRepository
{
	public interface IApplicationUserRepository
	{
		Task<ApplicationUser> GetUserByIdAsync(string userId);
		Task UpdateUserAsync(ApplicationUser user);
	}
}
