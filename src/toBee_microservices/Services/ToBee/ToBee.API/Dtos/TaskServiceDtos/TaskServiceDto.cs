using System.Text.Json.Serialization;
using ToBee.API.Models;

namespace ToBee.API.Dtos.TaskServiceDtos
{
	public class TaskServiceDto
	{
		public Guid TaskId { get; set; }
		public string TaskName { get; set; }
		public string TaskDescription { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public TaskPriority PriorityLevel { get; set; }
		public DateTime Deadline { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Models.TaskStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
		public string? UserId { get; set; }
	}
}
