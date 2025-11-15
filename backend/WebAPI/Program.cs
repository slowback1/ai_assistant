using WebAPI.Configuration;
using WebAPI.Config;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
CrudFactoryConfigurator.ConfigureCrudFactory(builder.Services, builder.Configuration);

builder.Services.Configure<AIConfig>(builder.Configuration.GetSection("AI"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AIConfig>>().Value);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();