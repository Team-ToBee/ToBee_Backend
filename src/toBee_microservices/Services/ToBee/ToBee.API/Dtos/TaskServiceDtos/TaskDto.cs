namespace ToBee.API.Dtos.TaskServiceDtos
{
	public class TaskDto
	{
		public string Category { get; set; }
		public string Description { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime DueDate { get; set; }
		public string Priority { get; set; }
		public int EstimatedDuration { get; set; }
	}
}
