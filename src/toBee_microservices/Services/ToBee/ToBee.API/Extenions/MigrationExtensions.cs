using Microsoft.EntityFrameworkCore;
using ToBee.API.Data;

namespace ToBee.API.Extenions
{
	public static class MigrationExtensions
	{
		public static void ApplyMigration(this IApplicationBuilder app)
		{
			using IServiceScope scope = app.ApplicationServices.CreateScope();
			using AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			context.Database.Migrate();
		}
	}
}
