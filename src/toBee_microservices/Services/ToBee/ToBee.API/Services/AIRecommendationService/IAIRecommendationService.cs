namespace ToBee.API.Services.AIRecommendationService
{
	using System.Net.Http;
	using System.Text;
	using System.Text.Json;
	using System.Threading.Tasks;

	namespace ToBee.API.Services.AIRecommendationService
	{
		public class AIRecommendationService : IAIRecommendationService
		{
			private readonly HttpClient _httpClient;

			public AIRecommendationService(HttpClient httpClient)
			{
				_httpClient = httpClient;
			}

			public async Task<AIRecommendationResponse> GetRecommendationsAsync(AIRecommendationRequest request)
			{
				var jsonRequest = JsonSerializer.Serialize(request);
				var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync("http://localhost:5000/predict", content);
				response.EnsureSuccessStatusCode();

				var jsonResponse = await response.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<AIRecommendationResponse>(jsonResponse);
			}
		}
	}

}
