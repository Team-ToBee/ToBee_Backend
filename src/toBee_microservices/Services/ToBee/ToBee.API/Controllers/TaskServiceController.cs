using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ToBee.API.Dtos.TaskServiceDtos;
using ToBee.API.Models;
using ToBee.API.Repositories.TaskServiceRepository;

namespace ToBee.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TaskServiceController : ControllerBase
	{
		private readonly ITaskServiceRepository _taskServiceRepository;
		private readonly IMapper _mapper;

		public TaskServiceController(ITaskServiceRepository taskServiceRepository, IMapper mapper)
		{
			_taskServiceRepository = taskServiceRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[SwaggerOperation(
			Summary = "Gets all tasks.",
			Description = "Returns a list of all tasks.",
			OperationId = "GetAllTasks",
			Tags = new[] { "TaskService" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "A list of all tasks.", typeof(IEnumerable<TaskServiceDto>))]
		public async Task<ActionResult<IEnumerable<TaskServiceDto>>> GetAllTasks()
		{
			var tasks = await _taskServiceRepository.GetAllTasksAsync();
			var taskDtos = _mapper.Map<IEnumerable<TaskServiceDto>>(tasks);
			return Ok(taskDtos);
		}

		[HttpGet("{id}")]
		[SwaggerOperation(
			Summary = "Gets a task by its ID.",
			Description = "Returns the task with the specified ID.",
			OperationId = "GetTaskById",
			Tags = new[] { "TaskService" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "The task with the specified ID.", typeof(TaskServiceDto))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Task not found.")]
		public async Task<ActionResult<TaskServiceDto>> GetTaskById(Guid id)
		{
			var task = await _taskServiceRepository.GetTaskByIdAsync(id);
			if (task == null)
			{
				return NotFound();
			}

			var taskDto = _mapper.Map<TaskServiceDto>(task);
			return Ok(taskDto);
		}

		[HttpPost]
		[SwaggerOperation(
			Summary = "Creates a new task.",
			Description = "Creates a new task and returns the created task.",
			OperationId = "CreateTask",
			Tags = new[] { "TaskService" }
		)]
		[SwaggerResponse(StatusCodes.Status201Created, "The created task.", typeof(TaskServiceDto))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input.")]
		public async Task<ActionResult<TaskServiceDto>> CreateTask(TaskServiceDto taskDto)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var taskService = _mapper.Map<TaskService>(taskDto);
			taskService.UserId = userId;

			await _taskServiceRepository.AddTaskAsync(taskService);

			var createdTaskDto = _mapper.Map<TaskServiceDto>(taskService);
			return CreatedAtAction(nameof(GetTaskById), new { id = createdTaskDto.TaskId }, createdTaskDto);
		}

		[HttpPut("{id}")]
		[SwaggerOperation(
			Summary = "Updates an existing task.",
			Description = "Updates the task with the specified ID.",
			OperationId = "UpdateTask",
			Tags = new[] { "TaskService" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Task updated successfully.")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input.")]
		public async Task<IActionResult> UpdateTask(Guid id, TaskServiceDto taskDto)
		{
			if (id != taskDto.TaskId)
			{
				return BadRequest();
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var taskService = _mapper.Map<TaskService>(taskDto);
			taskService.UserId = userId;

			await _taskServiceRepository.UpdateTaskAsync(taskService);

			return NoContent();
		}

		[HttpDelete("{id}")]
		[SwaggerOperation(
			Summary = "Deletes a task by its ID.",
			Description = "Deletes the task with the specified ID.",
			OperationId = "DeleteTask",
			Tags = new[] { "TaskService" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Task deleted successfully.")]
		public async Task<IActionResult> DeleteTask(Guid id)
		{
			await _taskServiceRepository.DeleteTaskAsync(id);
			return NoContent();
		}

		[HttpGet("daily")]
		[SwaggerOperation(
			Summary = "Gets all tasks for a specific day.",
			Description = "Returns a list of tasks for the specified day.",
			OperationId = "GetDailyTasks",
			Tags = new[] { "TaskService" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "A list of tasks for the specified day.", typeof(IEnumerable<TaskServiceDto>))]
		public async Task<ActionResult<IEnumerable<TaskServiceDto>>> GetDailyTasks([FromQuery] DateTime date)
		{
			var tasks = await _taskServiceRepository.GetDailyTasksAsync(date);
			var taskDtos = _mapper.Map<IEnumerable<TaskServiceDto>>(tasks);
			return Ok(taskDtos);
		}
	}
}
