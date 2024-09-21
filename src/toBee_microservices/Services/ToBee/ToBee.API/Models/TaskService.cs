using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToBee.API.Models
{
	public class TaskService
	{
		[Key]
		public Guid TaskId { get; set; }

		[Required]
		[StringLength(200)]
		public string TaskName { get; set; }

		[StringLength(1000)]
		public string TaskDescription { get; set; }

		[Required]
		public TaskPriority PriorityLevel { get; set; }

		public DateTime Deadline { get; set; }

		[Required]
		public TaskStatus Status { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime CompletedAt { get; set; }

		[Required]
		public string UserId { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
	}
}
