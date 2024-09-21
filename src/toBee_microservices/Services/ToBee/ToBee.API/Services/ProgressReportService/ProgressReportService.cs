using Microsoft.EntityFrameworkCore;
using ToBee.API.Data;
using ToBee.API.Models;
using ToBee.API.Repositories.RewardRepository;

namespace ToBee.API.Services.ProgressReportService
{
	public class ProgressReportService : IProgressReportService
	{
		private readonly AppDbContext _context;
		private readonly IRewardRepository _rewardRepository;

		public ProgressReportService(AppDbContext context, IRewardRepository rewardRepository)
		{
			_context = context;
			_rewardRepository = rewardRepository;
		}
		public async Task<ProgressReport> GenerateProgressReportAsync(string userId, DateTime startDate, DateTime endDate)
		{
			var totalFocusTime = await _context.PomodoroSessions
				.Where(ps => ps.UserId == userId && ps.StartTime >= startDate && ps.EndTime <= endDate)
				.SumAsync(ps => EF.Functions.DateDiffMinute(ps.StartTime, ps.EndTime) - ps.BreakDuration);

			var totalTasksCompleted = await _context.TaskServices
				.Where(ts => ts.UserId == userId && ts.Status == Models.TaskStatus.Completed && ts.CompletedAt >= startDate && ts.CompletedAt <= endDate)
				.CountAsync();

			var progressReport = new ProgressReport
			{
				ReportId = Guid.NewGuid(),
				UserId = userId,
				StartDate = startDate,
				EndDate = endDate,
				TotalFocusTime = totalFocusTime,
				TotalTasksCompleted = totalTasksCompleted,
				User = await _context.Users.FindAsync(userId)
			};

			return progressReport;
		}
	}
}
