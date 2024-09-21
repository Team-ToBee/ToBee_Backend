using Microsoft.AspNetCore.Mvc;
using ToBee.API.Services;
using System.Security.Claims;
using ToBee.API.Models;
using ToBee.API.Services.ProgressReportService;
using Microsoft.AspNetCore.Authorization;

namespace ToBee.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ProgressReportController : ControllerBase
	{
		private readonly IProgressReportService _progressReportService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ProgressReportController(IProgressReportService progressReportService, IHttpContextAccessor httpContextAccessor)
		{
			_progressReportService = progressReportService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet("user/progress-report")]
		public async Task<ActionResult<ProgressReportDto>> GetProgressReport(DateTime startDate, DateTime endDate)
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var progressReport = await _progressReportService.GenerateProgressReportAsync(userId, startDate, endDate);

			// Map ProgressReport to ProgressReportDto
			var progressReportDto = new ProgressReportDto
			{
				ReportId = progressReport.ReportId,
				UserId = progressReport.UserId,
				StartDate = progressReport.StartDate,
				EndDate = progressReport.EndDate,
				TotalFocusTime = progressReport.TotalFocusTime,
				TotalTasksCompleted = progressReport.TotalTasksCompleted
			};

			return Ok(progressReportDto);
		}
	}

	public class ProgressReportDto
	{
		public Guid ReportId { get; set; }
		public string UserId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public double TotalFocusTime { get; set; }
		public int TotalTasksCompleted { get; set; }
	}

}
