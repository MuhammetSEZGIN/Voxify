using System;
using Microsoft.EntityFrameworkCore;
using MessageService.Models;
namespace MessageService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
    .HasKey(u => u.Id);

        // Configure primary key for Message
        modelBuilder.Entity<Message>()
            .HasKey(m => m.Id);

        // Configure one-to-many relationship: one User has many Messages
        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
