using Microsoft.EntityFrameworkCore;
using OutlayService.Models;

namespace OutlayService.Data;

/// <summary>
/// Application database context for Entity Framework Core
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext class
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Users DbSet
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Configures the model for the database context
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(u => u.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(u => u.UpdatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            // Add index for Email for faster lookups
            entity.HasIndex(u => u.Email)
                .IsUnique();
        });
    }
}
