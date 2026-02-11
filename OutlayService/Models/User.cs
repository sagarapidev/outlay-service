using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OutlayService.Models
{
    public class User
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public long Id { get; set; }   // <-- changed to long

        [Required]
        [MaxLength(100)]
        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Column("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Column("created_on")]
        [JsonPropertyName("created_on")]
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        [Column("updated_on")]
        [JsonPropertyName("updated_on")]
        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}