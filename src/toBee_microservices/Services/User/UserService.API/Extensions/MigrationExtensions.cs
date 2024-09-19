using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using UserService.API.Data;

namespace UserService.API.Extensions
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
