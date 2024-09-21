using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToBee.API.Models
{
	public class AIRecommendation
	{
		[Key]
		public Guid RecommendationId { get; set; }

		[Required]
		public string UserId { get; set; }

		[Required]
		public Guid TaskId { get; set; }

		public DateTime GeneratedAt { get; set; } = DateTime.Now;

		public bool Applied { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }

		[ForeignKey("TaskId")]
		public TaskService TaskService { get; set; }
	}
}
