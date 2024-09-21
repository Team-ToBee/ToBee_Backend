using ToBee.API.Models;

namespace ToBee.API.Services.ProgressReportService
{
	public interface IProgressReportService
	{
		Task<ProgressReport> GenerateProgressReportAsync(string userId, DateTime startDate, DateTime endDate);
	}
}
