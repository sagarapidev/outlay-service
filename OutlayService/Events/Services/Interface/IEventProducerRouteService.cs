using OutlayService.Events.DTOs;

namespace OutlayService.Events.Services.Interface
{
    public interface IEventProducerRouteService
    {
        Task SendMessageAsync(string eventName, string message);

    }
}
