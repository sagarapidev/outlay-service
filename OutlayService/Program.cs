using Microsoft.EntityFrameworkCore;
using OutlayService.Constants;
using OutlayService.Data;
using OutlayService.Events.DTOs;
using OutlayService.Events.Services.Impl;
using OutlayService.Events.Services.Interface;
using OutlayService.Services.Impl;
using OutlayService.Services.Interfaces;
using Scalar.AspNetCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Load .env for local dev
EnvConfig.Load(builder.Configuration);

// Add EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IUserService, UserService>();

// Bind EVENTHUB_CONFIG (works with appsettings.json or env vars)
var eventHubOptions = new EventHubRouteOptions();
builder.Configuration.GetSection(AppConstant.EVENTHUB_CONFIG).Bind(eventHubOptions);

// Fallback: if Azure injects EVENTHUB_CONFIG as raw JSON string
if (eventHubOptions?.Routes == null || eventHubOptions.Routes.Count == 0)
{
    var eventHubConfigJson = builder.Configuration[AppConstant.EVENTHUB_CONFIG];
    if (!string.IsNullOrEmpty(eventHubConfigJson))
    {
        try
        {
            eventHubOptions = JsonSerializer.Deserialize<EventHubRouteOptions>(eventHubConfigJson);
        }
        catch (Exception ex)
        {
            builder.Services.AddSingleton<IEventProducerRouteService, NoOpEventProducerRouteService>();
            var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
            var logger = loggerFactory.CreateLogger("Startup");
            logger.LogError(ex, "Failed to parse EVENTHUB_CONFIG JSON string. EventHub integration disabled.");
        }
    }
}

// Validate routes
if (eventHubOptions?.Routes != null)
{
    eventHubOptions.Routes = eventHubOptions.Routes
        .Where(r => !string.IsNullOrWhiteSpace(r.ConnectionString) &&
                    !string.IsNullOrWhiteSpace(r.EventHubName))
        .ToList();
}

// Register Event Hub producer service
if (eventHubOptions?.Routes != null && eventHubOptions.Routes.Count > 0)
{
    builder.Services.AddSingleton<IEventProducerRouteService>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<EventProducerRouteService>>();
        return new EventProducerRouteService(eventHubOptions, logger);
    });
}
else
{
    builder.Services.AddSingleton<IEventProducerRouteService, NoOpEventProducerRouteService>();
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Startup logging
var startupLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

if (eventHubOptions?.Routes == null || eventHubOptions.Routes.Count == 0)
{
    startupLogger.LogWarning("EVENTHUB_CONFIG.Routes is missing or empty. EventHub integration disabled.");
}
else
{
    startupLogger.LogInformation("EVENTHUB_CONFIG successfully loaded with {Count} valid routes.", eventHubOptions.Routes.Count);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();