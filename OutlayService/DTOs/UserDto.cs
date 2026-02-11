using System.Text.Json.Serialization;

namespace OutlayService.DTOs
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }   // <-- changed to long

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("created_on")]
        public DateTime? CreatedOn { get; set; }

        [JsonPropertyName("updated_on")]
        public DateTime? UpdatedOn { get; set; }
    }
}