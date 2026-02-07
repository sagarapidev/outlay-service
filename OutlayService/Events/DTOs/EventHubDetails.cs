namespace OutlayService.Events.DTOs
{
    public class EventHubDetails
    {
        public string EventName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string EventHubName { get; set; } = string.Empty;
    }

}
