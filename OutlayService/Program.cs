using Microsoft.EntityFrameworkCore;
using OutlayService.Data;
using OutlayService.Services.Impl;
using OutlayService.Services.Interfaces;
using OutlayService.Events.DTOs;
using OutlayService.Events.Services.Interface;
using OutlayService.Events.Services.Impl;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Kestrel configuration will be read from appsettings.json

// Add EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IUserService, UserService>();

// Bind EventHubConfig section to EventHubRouteOptions
var eventHubOptions = new EventHubRouteOptions();
builder.Configuration.GetSection("EventHubConfig").Bind(eventHubOptions);

// Register Event Hub producer service
builder.Services.AddSingleton<IEventProducerRouteService>(
    new EventProducerRouteService(eventHubOptions)
);

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