using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace OutlayService.Controllers
{
    [ApiController]
    [Route("api/outlay-service/v1")]
    public class ConfigController(IConfiguration configuration, ILogger<ConfigController> logger) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<ConfigController> _logger = logger;

        [HttpGet("all")]
        public IActionResult GetAllConfig()
        {
            var configValues = new Dictionary<string, string>();

            foreach (var kvp in _configuration.AsEnumerable())
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    configValues[kvp.Key] = kvp.Value;
                }
            }

            // Serialize to pretty JSON
            var prettyJson = JsonSerializer.Serialize(configValues, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Log the pretty JSON (shows in console, App Service logs, and App Insights if enabled)
            _logger.LogInformation("App Configuration:\n{PrettyJson}", prettyJson);

            // Return JSON response
            return Content(prettyJson, "application/json");
        }
    }
}