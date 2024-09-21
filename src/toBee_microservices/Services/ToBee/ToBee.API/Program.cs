using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using ToBee.API.Data;
using ToBee.API.Extenions;
using ToBee.API.Models;
using ToBee.API.Repositories.ApplicationUserRepository;
using ToBee.API.Repositories.PomodoroSessionRepository;
using ToBee.API.Repositories.RewardRepository;
using ToBee.API.Repositories.TaskServiceRepository;
using ToBee.API.Services;
using ToBee.API.Services.AIRecommendationService;
using ToBee.API.Services.AIRecommendationService.ToBee.API.Services.AIRecommendationService;
using ToBee.API.Services.ProgressReportService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
options.SwaggerDoc("v1", new OpenApiInfo { Title = "ToBee API", Version = "v1" });

// Add security definition
options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
	In = ParameterLocation.Header,
	Description = "Please enter a valid token",
	Name = "Authorization",
	Type = SecuritySchemeType.Http,
	BearerFormat = "JWT",
	Scheme = "Bearer"
});

	// Add security requirement
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});


builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
	options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
	options.DefaultScheme = IdentityConstants.BearerScheme;
});

// Add this line to register authorization services
builder.Services.AddAuthorization();


builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
	.AddEntityFrameworkStores<AppDbContext>();

// add http client
builder.Services.AddHttpClient();

// Add Repos To conatiner 
builder.Services.AddScoped<ITaskServiceRepository, TaskServiceRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IPomodoroSessionRepository, PomodoroSessionRepository>();
builder.Services.AddScoped<PomodoroTimerService>();
builder.Services.AddScoped<IRewardRepository, RewardRepository>();
builder.Services.AddScoped<IProgressReportService, ProgressReportService>();
builder.Services.AddScoped<TaskMappingService>();
builder.Services.AddScoped<IAIRecommendationService, AIRecommendationService>();

// add cors
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		builder =>
		{
			builder.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader();
		});
});

// add db context

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// add automapper

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.

	app.UseSwagger();
	app.UseSwaggerUI();
	


app.MapGet("users/me", async (ClaimsPrincipal claims, AppDbContext context) =>
{
	string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
	var user = await context.Users.FindAsync( userId);
});


app.UseCors();
app.MapIdentityApi<ApplicationUser>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

