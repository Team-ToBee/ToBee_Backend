using Microsoft.AspNetCore.Mvc;
using ToBee.API.Services;
using ToBee.API.Repositories.TaskServiceRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ToBee.API.Services.AIRecommendationService;

namespace ToBee.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AIRecommendationController : ControllerBase
	{
		private readonly IAIRecommendationService _aiRecommendationService;
		private readonly ITaskServiceRepository _taskServiceRepository;
		private readonly TaskMappingService _taskMappingService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AIRecommendationController(
			IAIRecommendationService aiRecommendationService,
			ITaskServiceRepository taskServiceRepository,
			TaskMappingService taskMappingService,
			IHttpContextAccessor httpContextAccessor)
		{
			_aiRecommendationService = aiRecommendationService;
			_taskServiceRepository = taskServiceRepository;
			_taskMappingService = taskMappingService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet("user/recommendations")]
		public async Task<ActionResult<AIRecommendationResponse>> GetRecommendations()
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var tasks = await _taskServiceRepository.GetTasksByUserIdAsync(userId);
			var taskDtos = _taskMappingService.MapTasksToDto(tasks);

			var request = new AIRecommendationRequest { Tasks = taskDtos };
			var recommendations = await _aiRecommendationService.GetRecommendationsAsync(request);

			return Ok(recommendations);
		}
	}
}
