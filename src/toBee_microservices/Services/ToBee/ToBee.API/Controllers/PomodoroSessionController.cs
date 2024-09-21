using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ToBee.API.Models;
using ToBee.API.Repositories.PomodoroSessionRepository;
using ToBee.API.Services;

namespace ToBee.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PomodoroSessionController : ControllerBase
	{
		private readonly IPomodoroSessionRepository _sessionRepository;
		private readonly PomodoroTimerService _pomodoroTimerService;

		public PomodoroSessionController(IPomodoroSessionRepository sessionRepository, PomodoroTimerService pomodoroTimerService)
		{
			_sessionRepository = sessionRepository;
			_pomodoroTimerService = pomodoroTimerService;
		}

		/// <summary>
		/// Creates a new Pomodoro session.
		/// </summary>
		/// <param name="session">The Pomodoro session to create.</param>
		/// <returns>The created session.</returns>
		[HttpPost]
		[SwaggerOperation(
			Summary = "Creates a new Pomodoro session.",
			Description = "Creates a new Pomodoro session.",
			OperationId = "CreatePomodoroSession",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status201Created, "The created session.", typeof(PomodoroSession))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input.")]
		public async Task<ActionResult<PomodoroSession>> CreatePomodoroSession([FromBody] PomodoroSession session)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			session.SessionId = Guid.NewGuid();
			await _sessionRepository.CreateSessionAsync(session);

			return CreatedAtAction(nameof(GetPomodoroSessionById), new { id = session.SessionId }, session);
		}

		/// <summary>
		/// Gets a Pomodoro session by ID.
		/// </summary>
		/// <param name="id">The ID of the session.</param>
		/// <returns>The session.</returns>
		[HttpGet("{id}")]
		[SwaggerOperation(
			Summary = "Gets a Pomodoro session by ID.",
			Description = "Gets a Pomodoro session by ID.",
			OperationId = "GetPomodoroSessionById",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status200OK, "The session.", typeof(PomodoroSession))]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found.")]
		public async Task<ActionResult<PomodoroSession>> GetPomodoroSessionById(Guid id)
		{
			var session = await _sessionRepository.GetSessionByIdAsync(id);
			if (session == null)
			{
				return NotFound();
			}
			return Ok(session);
		}

		/// <summary>
		/// Updates a Pomodoro session.
		/// </summary>
		/// <param name="id">The ID of the session to update.</param>
		/// <param name="session">The updated session.</param>
		/// <returns>No content.</returns>
		[HttpPut("{id}")]
		[SwaggerOperation(
			Summary = "Updates a Pomodoro session.",
			Description = "Updates a Pomodoro session.",
			OperationId = "UpdatePomodoroSession",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Session updated successfully.")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found.")]
		public async Task<IActionResult> UpdatePomodoroSession(Guid id, [FromBody] PomodoroSession session)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var existingSession = await _sessionRepository.GetSessionByIdAsync(id);
			if (existingSession == null)
			{
				return NotFound();
			}

			existingSession.StartTime = session.StartTime;
			existingSession.EndTime = session.EndTime;
			existingSession.BreakDuration = session.BreakDuration;
			existingSession.Status = session.Status;

			await _sessionRepository.UpdateSessionAsync(existingSession);

			return NoContent();
		}

		/// <summary>
		/// Deletes a Pomodoro session.
		/// </summary>
		/// <param name="id">The ID of the session to delete.</param>
		/// <returns>No content.</returns>
		[HttpDelete("{id}")]
		[SwaggerOperation(
			Summary = "Deletes a Pomodoro session.",
			Description = "Deletes a Pomodoro session.",
			OperationId = "DeletePomodoroSession",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Session deleted successfully.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found.")]
		public async Task<IActionResult> DeletePomodoroSession(Guid id)
		{
			var session = await _sessionRepository.GetSessionByIdAsync(id);
			if (session == null)
			{
				return NotFound();
			}

			await _sessionRepository.DeleteSessionAsync(id);

			return NoContent();
		}

		/// <summary>
		/// Starts a Pomodoro session.
		/// </summary>
		/// <param name="id">The ID of the session to start.</param>
		/// <returns>No content.</returns>
		[HttpPost("{id}/start")]
		[SwaggerOperation(
			Summary = "Starts a Pomodoro session.",
			Description = "Starts a Pomodoro session.",
			OperationId = "StartPomodoroSession",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Session started successfully.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found.")]
		public async Task<IActionResult> StartPomodoroSession(Guid id)
		{
			try
			{
				await _pomodoroTimerService.StartSessionAsync(id);
				return NoContent();
			}
			catch (ArgumentException ex)
			{
				return NotFound(ex.Message);
			}
		}

		/// <summary>
		/// Pauses a Pomodoro session.
		/// </summary>
		/// <param name="id">The ID of the session to pause.</param>
		/// <returns>No content.</returns>
		[HttpPost("{id}/pause")]
		[SwaggerOperation(
			Summary = "Pauses a Pomodoro session.",
			Description = "Pauses a Pomodoro session.",
			OperationId = "PausePomodoroSession",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Session paused successfully.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found.")]
		public IActionResult PausePomodoroSession(Guid id)
		{
			try
			{
				_pomodoroTimerService.PauseSession();
				return NoContent();
			}
			catch (ArgumentException ex)
			{
				return NotFound(ex.Message);
			}
		}


		/// <summary>
		/// Resumes a Pomodoro session.
		/// </summary>
		/// <param name="id">The ID of the session to resume.</param>
		/// <returns>No content.</returns>
		[HttpPost("{id}/resume")]
		[SwaggerOperation(
			Summary = "Resumes a Pomodoro session.",
			Description = "Resumes a Pomodoro session.",
			OperationId = "ResumePomodoroSession",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Session resumed successfully.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found.")]
		public async Task<IActionResult> ResumePomodoroSession(Guid id)
		{
			try
			{
				await _pomodoroTimerService.ResumeSessionAsync(id);
				return NoContent();
			}
			catch (ArgumentException ex)
			{
				return NotFound(ex.Message);
			}
		}

		/// <summary>
		/// Stops a Pomodoro session.
		/// </summary>
		/// <param name="id">The ID of the session to stop.</param>
		/// <returns>No content.</returns>
		[HttpPost("{id}/stop")]
		[SwaggerOperation(
			Summary = "Stops a Pomodoro session.",
			Description = "Stops a Pomodoro session.",
			OperationId = "StopPomodoroSession",
			Tags = new[] { "PomodoroSession" }
		)]
		[SwaggerResponse(StatusCodes.Status204NoContent, "Session stopped successfully.")]
		[SwaggerResponse(StatusCodes.Status404NotFound, "Session not found.")]
		public async Task<IActionResult> StopPomodoroSession(Guid id)
		{
			try
			{
				await _pomodoroTimerService.StopSessionAsync(id);
				return NoContent();
			}
			catch (ArgumentException ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}
