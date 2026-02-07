using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using OutlayService.Events.DTOs;
using OutlayService.Events.Services.Interface;

namespace OutlayService.Events.Services.Impl
{
    public class EventProducerRouteService : IEventProducerRouteService
    {
        private readonly Dictionary<string, EventHubProducerClient> _producers;
        private readonly ILogger<EventProducerRouteService> _logger;

        public EventProducerRouteService(EventHubRouteOptions options, ILogger<EventProducerRouteService> logger)
        {
            _logger = logger;

            _producers = [];

            foreach (var route in options.Routes)
            {
                _logger.LogInformation("Configuring Event Hub route: EventName={EventName}, Hub={EventHubName}",
                    route.EventName, route.EventHubName);

                _producers[route.EventName] = new EventHubProducerClient(
                    route.ConnectionString,
                    route.EventHubName
                );
            }

           
        }

        public async Task SendMessageAsync(string eventName, string message)
        {
            if (!_producers.ContainsKey(eventName))
            {
                _logger.LogError("No Event Hub configured for event '{EventName}'", eventName);
                throw new KeyNotFoundException($"No Event Hub configured for event '{eventName}'.");
            }

            var producer = _producers[eventName];
            _logger.LogInformation("Sending message to Event Hub: EventName={EventName}, Length={Length}",
                eventName, message.Length);

            try
            {
                using EventDataBatch eventBatch = await producer.CreateBatchAsync();
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message))))
                {
                    _logger.LogWarning("Message too large to fit in batch for EventName={EventName}", eventName);
                }
                await producer.SendAsync(eventBatch);
                _logger.LogInformation("Message successfully sent to Event Hub '{EventName}'", eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to Event Hub '{EventName}'", eventName);
                throw;
            }

        }
    }
}