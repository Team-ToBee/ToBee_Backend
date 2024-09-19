using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.API.Data;
using UserService.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
	.AddGoogle(options =>
	{
		var googleAuth = builder.Configuration.GetSection("Authentication:Google");
		options.ClientId = googleAuth["ClientId"];
		options.ClientSecret = googleAuth["ClientSecret"];
	})
	.AddFacebook(options =>
	{
		var facebookAuth = builder.Configuration.GetSection("Authentication:Facebook");
		options.AppId = facebookAuth["AppId"];
		options.AppSecret = facebookAuth["AppSecret"];
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
