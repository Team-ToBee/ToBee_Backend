using ToBee.API.Models;

namespace ToBee.API.Dtos.TaskServiceDtos
{
	public class TaskServiceDto
	{
		public Guid TaskId { get; set; }
		public string TaskName { get; set; }
		public string TaskDescription { get; set; }
		public TaskPriority PriorityLevel { get; set; }
		public DateTime Deadline { get; set; }
		public Models.TaskStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime CompletedAt { get; set; }
		public string UserId { get; set; }
	}
}
