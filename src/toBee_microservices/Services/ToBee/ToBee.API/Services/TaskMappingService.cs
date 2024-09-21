using System;
using System.Collections.Generic;
using System.Linq;
using ToBee.API.Dtos.TaskServiceDtos;
using ToBee.API.Models;

namespace ToBee.API.Services
{
	public class TaskMappingService
	{
		public List<TaskDto> MapTasksToDto(List<TaskService> tasks)
		{
			return tasks.Select(task => new TaskDto
			{
				Category = task.TaskName, 
				Description = task.TaskDescription,
				CreationDate = task.CreatedAt,
				DueDate = task.Deadline,
				Priority = task.PriorityLevel.ToString(),
				EstimatedDuration = (int)(task.Deadline - task.CreatedAt).TotalMinutes 
			}).ToList();
		}
	}
}
