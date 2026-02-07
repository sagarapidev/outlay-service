using Microsoft.AspNetCore.Mvc;
using OutlayService.Events.Services.Interface;
using System.Text.Json;
using System.Threading.Tasks;

namespace OutlayService.Controllers
{
    [ApiController]
    [Route("api/eventhub")]
    public class EventhubController(IEventProducerRouteService producerService) : ControllerBase
    {
        private readonly IEventProducerRouteService _producerService = producerService;

        // Example: POST /api/eventhub/OutlayEvent/send
        [HttpPost("{eventName}/send")]
        public async Task<IActionResult> Send(string eventName, [FromBody] object payload)
        {
            string message = JsonSerializer.Serialize(payload);

            await _producerService.SendMessageAsync(eventName, message);

            return Ok(new
            {
                status = $"Message sent to Event Hub route '{eventName}'"
            });
        }
    }
}