using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToBee.API.Models
{
	public class PomodoroSession
	{
		[Key]
		public Guid SessionId { get; set; }

		[Required]
		public string UserId { get; set; }

		[Required]
		public Guid TaskId { get; set; }

		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		[Range(0, int.MaxValue)]
		public int BreakDuration { get; set; }

		[Required]
		public string Status { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }

		[ForeignKey("TaskId")]
		public TaskService TaskService { get; set; }
	}
}
