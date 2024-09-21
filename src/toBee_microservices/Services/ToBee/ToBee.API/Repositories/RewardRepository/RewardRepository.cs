using Microsoft.EntityFrameworkCore;
using ToBee.API.Data;
using ToBee.API.Models;

namespace ToBee.API.Repositories.RewardRepository
{
	public class RewardRepository : IRewardRepository
	{
		private readonly AppDbContext _context;

		public RewardRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Reward>> GetAllRewardsForUserAsync(string userId)
		{
			return await _context.Rewards.Where(r => r.UserId == userId).ToListAsync();
		}

		public async Task<int> GetTotalPointsForUserAsync(string userId)
		{
			return await _context.Rewards.Where(r => r.UserId == userId).SumAsync(r => r.Points);
		}

		public async Task<string> GetBadgeForUserAsync(string userId)
		{
			return await _context.Rewards.Where(r => r.UserId == userId)
										 .OrderByDescending(r => r.DateEarned)
										 .Select(r => r.BadgeEarned)
										 .FirstOrDefaultAsync();
		}
	}
}
