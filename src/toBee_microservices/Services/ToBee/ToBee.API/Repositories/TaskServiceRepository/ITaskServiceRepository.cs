using ToBee.API.Models;

namespace ToBee.API.Repositories.TaskServiceRepository
{
	public interface ITaskServiceRepository
	{
		Task<IEnumerable<TaskService>> GetAllTasksAsync();
		Task<TaskService> GetTaskByIdAsync(Guid taskId);

		Task<List<TaskService>> GetTasksByUserIdAsync(string userId);
	    Task AddTaskAsync(TaskService taskService);
		Task UpdateTaskAsync(TaskService taskService);
		Task DeleteTaskAsync(Guid taskId);
		Task<IEnumerable<TaskService>> GetDailyTasksAsync(DateTime date);
		Task<IEnumerable<TaskService>> GetAllTasksOrderedAsync();
		Task<IEnumerable<TaskService>> GetFilteredTasksAsync(Models.TaskStatus? status, TaskPriority? priority, DateTime? dateCreated, string name, DateTime? deadline, DateTime? completedAt);
	}
}
