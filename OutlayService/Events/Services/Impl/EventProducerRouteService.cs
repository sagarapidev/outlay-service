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

        public EventProducerRouteService(EventHubRouteOptions options)
        {
            _producers = [];

            foreach (var route in options.Routes)
            {
                _producers[route.EventName] = new EventHubProducerClient(
                    route.ConnectionString,
                    route.EventHubName
                );
            }
        }

        public async Task SendMessageAsync(string eventName, string message)
        {
            if (!_producers.ContainsKey(eventName))
                throw new KeyNotFoundException($"No Event Hub configured for event '{eventName}'.");

            var producer = _producers[eventName];
            using EventDataBatch eventBatch = await producer.CreateBatchAsync();
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message)));
            await producer.SendAsync(eventBatch);
        }
    }
}