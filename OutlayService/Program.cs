using Microsoft.EntityFrameworkCore;
using OutlayService.Data;
using OutlayService.Services.Impl;
using OutlayService.Services.Interfaces;
using OutlayService.Events.DTOs;
using OutlayService.Events.Services.Interface;
using OutlayService.Events.Services.Impl;
using Scalar.AspNetCore;
using OutlayService.Constants;

var builder = WebApplication.CreateBuilder(args);

// Kestrel configuration will be read from appsettings.json

// Add EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IUserService, UserService>();

// Bind EventHubConfig section to EventHubRouteOptions
var eventHubOptions = new EventHubRouteOptions();
builder.Configuration.GetSection(AppConstant.EVENTHUB_CONFIG).Bind(eventHubOptions);

// Validate configuration to avoid null reference warnings
if (eventHubOptions?.Routes == null || eventHubOptions.Routes.Count == 0)
{
    throw new InvalidOperationException("EventHubConfig.Routes is missing or empty in configuration.");
}

// Register Event Hub producer service with logger injection
builder.Services.AddSingleton<IEventProducerRouteService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<EventProducerRouteService>>();
    return new EventProducerRouteService(eventHubOptions, logger);
});

builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddOpenApi();

// Logging
builder.Services.AddLogging();

// CORS
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

try
{
    app.MapControllers();
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    foreach (var typeLoadException in ex.LoaderExceptions)
    {
        Console.WriteLine($"Type loading failed: {typeLoadException.Message}");
    }
    throw;
}

app.Run();