using Microsoft.EntityFrameworkCore;
using ClanService.Models;

namespace ClanService.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<ClanMembership> ClanMemberships { get; set; }
        public DbSet<VoiceChannel> VoiceChannels { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Channel>().
                HasOne(c => c.Clan).
                WithMany(c => c.Channels).
                HasForeignKey(c => c.ClanId).
                OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<ClanMembership>().
                HasOne(cm => cm.Clan).
                WithMany(c => c.ClanMemberShips).
                HasForeignKey(cm => cm.ClanId).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClanMembership>().
                HasOne(cm => cm.User).
                WithMany(u => u.ClanMemberships).
                HasForeignKey(cm => cm.UserId).
                OnDelete(DeleteBehavior.Cascade);
        }
    }
}
