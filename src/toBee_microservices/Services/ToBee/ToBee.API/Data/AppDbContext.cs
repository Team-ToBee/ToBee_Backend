using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToBee.API.Models;

namespace ToBee.API.Data
{
	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<TaskService> TaskServices { get; set; }
		public DbSet<ProgressReport> ProgressReports { get; set; }
		public DbSet<PomodoroSession> PomodoroSessions { get; set; }
		public DbSet<AIRecommendation> AIRecommendations { get; set; }
		public DbSet<Reward> Rewards { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			
		}
	}
}
