using Microsoft.EntityFrameworkCore;
using OutlayService.Data;
using OutlayService.Services.Impl;
using OutlayService.Services.Interfaces;
using OutlayService.Events.DTOs;
using OutlayService.Events.Services.Interface;
using OutlayService.Events.Services.Impl;
using Scalar.AspNetCore;
using OutlayService.Constants;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IUserService, UserService>();

// Read EVENTHUB_CONFIG as raw JSON string
var eventHubConfigJson = builder.Configuration[AppConstant.EVENTHUB_CONFIG];
EventHubRouteOptions? eventHubOptions = null;

if (!string.IsNullOrEmpty(eventHubConfigJson))
{
    try
    {
        eventHubOptions = JsonSerializer.Deserialize<EventHubRouteOptions>(eventHubConfigJson);
        if (eventHubOptions?.Routes != null && eventHubOptions.Routes.Count > 0)
        {
            // Register Event Hub producer service with logger injection
            builder.Services.AddSingleton<IEventProducerRouteService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventProducerRouteService>>();
                return new EventProducerRouteService(eventHubOptions, logger);
            });
        }
    }
    catch (Exception ex)
    {
        // Log parse error later after app is built
        builder.Services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
            logger.LogError(ex, "Failed to parse EVENTHUB_CONFIG JSON. EventHub integration disabled.");
            return new object(); // dummy registration to satisfy DI
        });
    }
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

if (string.IsNullOrEmpty(eventHubConfigJson))
{
    startupLogger.LogWarning("EVENTHUB_CONFIG is missing or empty. EventHub integration disabled.");
}
else if (eventHubOptions?.Routes == null || eventHubOptions.Routes.Count == 0)
{
    startupLogger.LogWarning("EVENTHUB_CONFIG.Routes is missing or empty. EventHub integration disabled.");
}
else
{
    startupLogger.LogInformation("EVENTHUB_CONFIG successfully loaded with {Count} routes.", eventHubOptions.Routes.Count);
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