using System;
using System.Threading.Tasks;
using OutlayService.Events.Services.Interface;

namespace OutlayService.Events.Services.Impl
{
    /// <summary>
    /// Fallback implementation when EVENTHUB_CONFIG is missing or invalid.
    /// Prevents DI resolution errors and logs instead of sending.
    /// </summary>
    public class NoOpEventProducerRouteService : IEventProducerRouteService
    {
        public Task SendMessageAsync(string eventName, string message)
        {
            Console.WriteLine($"[NoOp] EventHub disabled. Tried to send event '{eventName}' with payload: {message}");
            return Task.CompletedTask;
        }
    }
}