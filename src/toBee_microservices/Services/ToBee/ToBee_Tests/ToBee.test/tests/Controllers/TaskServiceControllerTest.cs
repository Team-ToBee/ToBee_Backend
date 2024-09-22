using AutoMapper;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ToBee.API.Controllers;
using ToBee.API.Dtos.TaskServiceDtos;
using ToBee.API.Models;
using ToBee.API.Repositories.TaskServiceRepository;
using Xunit;

namespace ToBee.API.Tests.Controllers
{
    public class TaskServiceControllerTest
    {
        private readonly Mock<ITaskServiceRepository> _taskServiceRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TaskServiceController _controller;

        public TaskServiceControllerTest()
        {
            _taskServiceRepositoryMock = new Mock<ITaskServiceRepository>();
            _mapperMock = new Mock<IMapper>();
            _controller = new TaskServiceController(_taskServiceRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllTasks_ReturnsOkResult_WithListOfTasks()
        {
            // Arrange
            var tasks = new List<TaskService> { new TaskService() };
            var taskDtos = new List<TaskServiceDto> { new TaskServiceDto() };
            _taskServiceRepositoryMock.Setup(repo => repo.GetAllTasksAsync()).ReturnsAsync(tasks);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<TaskServiceDto>>(tasks)).Returns(taskDtos);

            // Act
            var result = await _controller.GetAllTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<TaskServiceDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetTaskById_ReturnsOkResult_WithTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskService { TaskId = taskId };
            var taskDto = new TaskServiceDto { TaskId = taskId };
            _taskServiceRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(task);
            _mapperMock.Setup(mapper => mapper.Map<TaskServiceDto>(task)).Returns(taskDto);

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<TaskServiceDto>(okResult.Value);
            Assert.Equal(taskId, returnValue.TaskId);
        }

        [Fact]
        public async Task GetTaskById_ReturnsNotFound_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskServiceRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync((TaskService)null);

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreatedAtAction_WithCreatedTask()
        {
            // Arrange
            var taskDto = new TaskServiceDto { TaskId = Guid.NewGuid() };
            var task = new TaskService { TaskId = taskDto.TaskId };
            _mapperMock.Setup(mapper => mapper.Map<TaskService>(taskDto)).Returns(task);
            _mapperMock.Setup(mapper => mapper.Map<TaskServiceDto>(task)).Returns(taskDto);
            _taskServiceRepositoryMock.Setup(repo => repo.AddTaskAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateTask(taskDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<TaskServiceDto>(createdAtActionResult.Value);
            Assert.Equal(taskDto.TaskId, returnValue.TaskId);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNoContent()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var taskDto = new TaskServiceDto { TaskId = taskId };
            var task = new TaskService { TaskId = taskId };
            _mapperMock.Setup(mapper => mapper.Map<TaskService>(taskDto)).Returns(task);
            _taskServiceRepositoryMock.Setup(repo => repo.UpdateTaskAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTask(taskId, taskDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTask_ReturnsOkResult()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskService { TaskId = taskId };
            _taskServiceRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(task);
            _taskServiceRepositoryMock.Setup(repo => repo.DeleteTaskAsync(taskId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("task deleted successfully", okResult.Value);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskServiceRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync((TaskService)null);

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetDailyTasks_ReturnsOkResult_WithListOfTasks()
        {
            // Arrange
            var date = DateTime.Today;
            var tasks = new List<TaskService> { new TaskService() };
            var taskDtos = new List<TaskServiceDto> { new TaskServiceDto() };
            _taskServiceRepositoryMock.Setup(repo => repo.GetDailyTasksAsync(date)).ReturnsAsync(tasks);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<TaskServiceDto>>(tasks)).Returns(taskDtos);

            // Act
            var result = await _controller.GetDailyTasks(date);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<TaskServiceDto>>(okResult.Value);
            Assert.Single(returnValue);
        }
    }
}