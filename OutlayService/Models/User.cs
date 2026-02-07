namespace OutlayService.Models;

/// <summary>
/// Represents a User entity in the system
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// User email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the user record was created
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the user record was last updated
    /// </summary>
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
}
