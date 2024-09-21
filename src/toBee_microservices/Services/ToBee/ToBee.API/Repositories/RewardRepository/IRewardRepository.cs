using ToBee.API.Models;

namespace ToBee.API.Repositories.RewardRepository
{
	public interface IRewardRepository
	{
		Task<IEnumerable<Reward>> GetAllRewardsForUserAsync(string userId);
		Task<int> GetTotalPointsForUserAsync(string userId);
		Task<string> GetBadgeForUserAsync(string userId);
	}

}
