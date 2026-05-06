using Microsoft.EntityFrameworkCore;
using VersionControlService.Models;

namespace VersionControlService.Data;

/// <summary>
/// Database context for version control release management.
/// </summary>
public class VersionControlDbContext : DbContext
{
    public VersionControlDbContext(DbContextOptions<VersionControlDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Software releases with platform-specific artifacts
    /// </summary>
    public DbSet<ReleaseEntity> Releases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Release entity
        var releaseBuilder = modelBuilder.Entity<ReleaseEntity>();

        releaseBuilder.ToTable("Releases");
        
        releaseBuilder.HasKey(r => r.Id);
        
        // Unique constraint on Version
        releaseBuilder.HasIndex(r => r.Version).IsUnique();
        
        // Version is required
        releaseBuilder.Property(r => r.Version)
            .IsRequired()
            .HasMaxLength(20);

        // Notes is optional
        releaseBuilder.Property(r => r.Notes)
            .HasMaxLength(2000);

        // Owned entity for artifacts
        releaseBuilder.OwnsMany(r => r.Artifacts, ab =>
        {
            ab.ToTable("ReleaseArtifacts");
            ab.WithOwner().HasForeignKey("ReleaseId");
            ab.HasKey("Id");

            ab.HasIndex("ReleaseId", nameof(ReleaseArtifactEntity.Target)).IsUnique();
            
            ab.Property(a => a.Target)
                .IsRequired()
                .HasMaxLength(50);

            ab.Property(a => a.Signature)
                .IsRequired()
                .HasMaxLength(500);

            ab.Property(a => a.Url)
                .IsRequired()
                .HasMaxLength(500);
        });

        releaseBuilder.Property(r => r.PubDate)
            .HasColumnType("TEXT"); // SQLite datetime

        releaseBuilder.Property(r => r.IsLatest)
            .HasDefaultValue(false);
    }
}
