using ToBee.API.Dtos.TaskServiceDtos;

namespace ToBee.API.Services.AIRecommendationService
{
	public interface IAIRecommendationService
	{
		Task<AIRecommendationResponse> GetRecommendationsAsync(AIRecommendationRequest request);
	}

	public class AIRecommendationRequest
	{
		public List<TaskDto> Tasks { get; set; }
	}

	public class AIRecommendationResponse
	{
		public List<RecommendationDto> Recommendations { get; set; }
	}


	public class RecommendationDto
	{
		public double CompletionProbability { get; set; }
		public string RecommendedPriority { get; set; }
		public string Task { get; set; }
	}
}
