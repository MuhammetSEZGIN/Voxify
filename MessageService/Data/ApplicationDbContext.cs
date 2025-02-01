using System;
using Microsoft.EntityFrameworkCore;
using MessageService.Models;
namespace MessageService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Clan> Clans { get; set; }
    public DbSet<ClanMemberShip> ClanMemberShips { get; set; }
}
