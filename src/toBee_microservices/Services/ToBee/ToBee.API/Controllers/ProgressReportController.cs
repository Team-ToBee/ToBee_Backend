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
		public async Task<ActionResult<ProgressReport>> GetProgressReport(DateTime startDate, DateTime endDate)
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized();
			}

			var progressReport = await _progressReportService.GenerateProgressReportAsync(userId, startDate, endDate);
			return Ok(progressReport);
		}
	}
}
