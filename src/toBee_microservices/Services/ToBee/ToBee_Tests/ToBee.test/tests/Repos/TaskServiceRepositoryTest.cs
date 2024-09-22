using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using ToBee.API.Data;
using ToBee.API.Models;
using ToBee.API.Repositories.TaskServiceRepository;
using Xunit;

namespace ToBee.API.Tests.Repos
{
    public class TaskServiceRepositoryTest
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly TaskServiceRepository _repository;

        public TaskServiceRepositoryTest()
        {
            _mockContext = new Mock<AppDbContext>();
            _repository = new TaskServiceRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetAllTasksAsync_ReturnsAllTasks()
        {
            // Arrange
            var tasks = new List<TaskService>
            {
                new TaskService { TaskId = Guid.NewGuid(), TaskName = "Task 1" },
                new TaskService { TaskId = Guid.NewGuid(), TaskName = "Task 2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TaskService>>();
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.Provider).Returns(tasks.Provider);
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.Expression).Returns(tasks.Expression);
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.ElementType).Returns(tasks.ElementType);
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.GetEnumerator()).Returns(tasks.GetEnumerator());

            _mockContext.Setup(c => c.TaskServices).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetAllTasksAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskService { TaskId = taskId, TaskName = "Task 1" };

            var mockSet = new Mock<DbSet<TaskService>>();
            mockSet.Setup(m => m.FindAsync(taskId)).ReturnsAsync(task);

            _mockContext.Setup(c => c.TaskServices).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetTaskByIdAsync(taskId);

            // Assert
            Assert.Equal(taskId, result.TaskId);
        }

        [Fact]
        public async Task AddTaskAsync_AddsTask()
        {
            // Arrange
            var task = new TaskService { TaskId = Guid.NewGuid(), TaskName = "Task 1" };

            var mockSet = new Mock<DbSet<TaskService>>();
            _mockContext.Setup(c => c.TaskServices).Returns(mockSet.Object);

            // Act
            await _repository.AddTaskAsync(task);

            // Assert
            mockSet.Verify(m => m.AddAsync(task, default), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateTaskAsync_UpdatesTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = new TaskService { TaskId = taskId, TaskName = "Task 1", Status = Models.TaskStatus.Delayed };
            var updatedTask = new TaskService { TaskId = taskId, TaskName = "Updated Task", Status = Models.TaskStatus.Done };

            var mockSet = new Mock<DbSet<TaskService>>();
            mockSet.Setup(m => m.FindAsync(taskId)).ReturnsAsync(existingTask);

            _mockContext.Setup(c => c.TaskServices).Returns(mockSet.Object);

            // Act
            await _repository.UpdateTaskAsync(updatedTask);

            // Assert
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            Assert.Equal("Updated Task", existingTask.TaskName);
            Assert.Equal(Models.TaskStatus.Done, existingTask.Status);
        }

        [Fact]
        public async Task DeleteTaskAsync_DeletesTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskService { TaskId = taskId, TaskName = "Task 1" };

            var mockSet = new Mock<DbSet<TaskService>>();
            mockSet.Setup(m => m.FindAsync(taskId)).ReturnsAsync(task);

            _mockContext.Setup(c => c.TaskServices).Returns(mockSet.Object);

            // Act
            await _repository.DeleteTaskAsync(taskId);

            // Assert
            mockSet.Verify(m => m.Remove(task), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task GetTasksByUserIdAsync_ReturnsTasks()
        {
            // Arrange
            var userId = "user1";
            var tasks = new List<TaskService>
            {
                new TaskService { TaskId = Guid.NewGuid(), TaskName = "Task 1", UserId = userId },
                new TaskService { TaskId = Guid.NewGuid(), TaskName = "Task 2", UserId = userId }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TaskService>>();
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.Provider).Returns(tasks.Provider);
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.Expression).Returns(tasks.Expression);
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.ElementType).Returns(tasks.ElementType);
            mockSet.As<IQueryable<TaskService>>().Setup(m => m.GetEnumerator()).Returns(tasks.GetEnumerator());

            _mockContext.Setup(c => c.TaskServices).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetTasksByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count);
        }
    }
}