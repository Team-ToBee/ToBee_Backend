using Microsoft.EntityFrameworkCore;
using ToBee.API.Data;
using ToBee.API.Models;

namespace ToBee.API.Repositories.TaskServiceRepository
{
	public class TaskServiceRepository : ITaskServiceRepository
	{
		private readonly AppDbContext _context;

		public TaskServiceRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<TaskService>> GetAllTasksAsync()
		{
			return await _context.TaskServices.ToListAsync();
		}

		public async Task<TaskService> GetTaskByIdAsync(Guid taskId)
		{
			return await _context.TaskServices.FindAsync(taskId);
		}

		public async Task<List<TaskService>> GetTasksByUserIdAsync(string userId)
		{
			return await _context.TaskServices
				.Where(ts => ts.UserId == userId)
				.ToListAsync();
		}

		public async Task AddTaskAsync(TaskService taskService)
		{
			await _context.TaskServices.AddAsync(taskService);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateTaskAsync(TaskService taskService)
		{
			var existingTask = await _context.TaskServices.FindAsync(taskService.TaskId);
			if (existingTask != null)
			{
				// Check if the status is changed to Completed
				if (existingTask.Status != Models.TaskStatus.Completed && taskService.Status == Models.TaskStatus.Completed)
				{
					await CreateRewardAsync(taskService);
				}

				_context.Entry(existingTask).CurrentValues.SetValues(taskService);
				await _context.SaveChangesAsync();
			}
		}

		public async Task DeleteTaskAsync(Guid taskId)
		{
			var taskService = await _context.TaskServices.FindAsync(taskId);
			if (taskService != null)
			{
				_context.TaskServices.Remove(taskService);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<TaskService>> GetDailyTasksAsync(DateTime date)
		{
			return await _context.TaskServices
				.Where(t => t.Deadline.Date == date.Date)
				.ToListAsync();
		}

		public async Task<IEnumerable<TaskService>> GetAllTasksOrderedAsync()
		{
			return await _context.TaskServices
				.OrderBy(t => t.Status)
				.ThenByDescending(t => t.Deadline)
				.ToListAsync();
		}

		public async Task<IEnumerable<TaskService>> GetFilteredTasksAsync(Models.TaskStatus? status, TaskPriority? priority, DateTime? dateCreated, string name, DateTime? deadline, DateTime? completedAt)
		{
			var query = _context.TaskServices.AsQueryable();

			if (status.HasValue)
			{
				query = query.Where(t => t.Status == status.Value);
			}

			if (priority.HasValue)
			{
				query = query.Where(t => t.PriorityLevel == priority.Value);
			}

			if (dateCreated.HasValue)
			{
				query = query.Where(t => t.CreatedAt.Date == dateCreated.Value.Date);
			}

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(t => t.TaskName.Contains(name));
			}

			if (deadline.HasValue)
			{
				query = query.Where(t => t.Deadline.Date == deadline.Value.Date);
			}

			if (completedAt.HasValue)
			{
				query = query.Where(t => t.CompletedAt.Date == completedAt.Value.Date);
			}

			return await query.ToListAsync();
		}

		private async Task CreateRewardAsync(TaskService taskService)
		{
			var points = CalculatePoints(taskService);
			var badge = DetermineBadge(points);

			var reward = new Reward
			{
				RewardId = Guid.NewGuid(),
				UserId = taskService.UserId,
				Points = points,
				BadgeEarned = badge,
				DateEarned = DateTime.UtcNow
			};

			await _context.Rewards.AddAsync(reward);
			await _context.SaveChangesAsync();
		}

		private int CalculatePoints(TaskService task)
		{
			
			return task.PriorityLevel switch
			{
				TaskPriority.Low => 10,
				TaskPriority.Medium => 20,
				TaskPriority.High => 30,
				_ => 0
			};
		}

		private string DetermineBadge(int points)
		{
			
			if (points >= 100)
				return "Gold";
			if (points >= 50)
				return "Silver";
			if (points >= 20)
				return "Bronze";
			return "Participant";
		}
	}
}
