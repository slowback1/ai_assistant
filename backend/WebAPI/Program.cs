using Common.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Quartz;
using WebAPI.Configuration;
using WebAPI.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(new CorsPolicy
	{
		Origins = { "*" },
		Headers = { "*" },
		Methods = { "*" }
	});
});
CrudFactoryConfigurator.ConfigureCrudFactory(builder.Services, builder.Configuration);

builder.Services.Configure<AIConfig>(builder.Configuration.GetSection("AI"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AIConfig>>().Value);

// Configure Quartz for scheduled jobs only if not in test environment
var isTestEnvironment = builder.Environment.EnvironmentName == "Test";
if (!isTestEnvironment)
{
	builder.Services.AddQuartz(q =>
	{
		// Create a job key for our story generation job
		var jobKey = new JobKey("StoryGenerationJob");
		
		q.AddJob<StoryGenerationJob>(opts => opts.WithIdentity(jobKey));
		
		// Create a trigger that runs every hour
		q.AddTrigger(opts => opts
			.ForJob(jobKey)
			.WithIdentity("StoryGenerationJob-trigger")
			.WithCronSchedule("0 0 * * * ?") // Every hour on the hour
		);
	});

	// Add Quartz hosted service
	builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();