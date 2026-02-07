using Microsoft.AspNetCore.Mvc;
using OutlayService.Events.Services.Interface;
using System.Text.Json;
using System.Threading.Tasks;

namespace OutlayService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OutlayEvtProducerController(IEventProducerRouteService producerService) : ControllerBase
    {
        private readonly IEventProducerRouteService _producerService = producerService;

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] object payload)
        {
            string message = JsonSerializer.Serialize(payload);
            await _producerService.SendMessageAsync("OutlayEvent", message);
            return Ok(new { status = "Message sent to Event Hub" });
        }
    }
}