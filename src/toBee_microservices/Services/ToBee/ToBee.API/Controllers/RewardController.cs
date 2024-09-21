using Microsoft.AspNetCore.Mvc;
using ToBee.API.Repositories.RewardRepository;
using ToBee.API.Dtos.RewardDtos;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ToBee.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class RewardController : ControllerBase
	{
		private readonly IRewardRepository _rewardRepository;
		private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public RewardController(IRewardRepository rewardRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
		{
			_rewardRepository = rewardRepository;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet("user")]
		[SwaggerOperation(
			Summary = "Gets all rewards for the authenticated user.",
			Description = "Returns a list of all rewards for the authenticated user.",
			OperationId = "GetAllRewardsForUser",
			Tags = new[] { "Reward" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "A list of all rewards for the user.", typeof(IEnumerable<RewardDto>))]
		public async Task<ActionResult<IEnumerable<RewardDto>>> GetAllRewardsForUser()
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var rewards = await _rewardRepository.GetAllRewardsForUserAsync(userId);
			var rewardDtos = _mapper.Map<IEnumerable<RewardDto>>(rewards);
			return Ok(rewardDtos);
		}

		[HttpGet("user/total-points")]
		[SwaggerOperation(
			Summary = "Gets the total points for the authenticated user.",
			Description = "Returns the total points for the authenticated user.",
			OperationId = "GetTotalPointsForUser",
			Tags = new[] { "Reward" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "The total points for the user.", typeof(int))]
		public async Task<ActionResult<int>> GetTotalPointsForUser()
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var totalPoints = await _rewardRepository.GetTotalPointsForUserAsync(userId);
			return Ok(totalPoints);
		}

		[HttpGet("user/badge")]
		[SwaggerOperation(
			Summary = "Gets the latest badge for the authenticated user.",
			Description = "Returns the latest badge for the authenticated user.",
			OperationId = "GetBadgeForUser",
			Tags = new[] { "Reward" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "The latest badge for the user.", typeof(string))]
		public async Task<ActionResult<string>> GetBadgeForUser()
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var badge = await _rewardRepository.GetBadgeForUserAsync(userId);
			return Ok(badge);
		}
	}
}
