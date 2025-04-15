using System.Threading.Tasks;
using ClanService.Data;
using ClanService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClanService.Repositories.Tests;

[TestClass]
public class UserRepositoryTest
{
    private UserRepository _userRepository;
    private ApplicationDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        // Clear the database before each test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();  

        _userRepository = new UserRepository(_context);
    }

    [TestMethod]
    public async Task GetUsersByClanIdAsync_ShouldResturnUsers_ByValidUser()
    {
        var userId1 = Guid.NewGuid().ToString();
        var userId2 = Guid.NewGuid().ToString();
        var userId3 = Guid.NewGuid().ToString();
        var clanId = Guid.NewGuid();
        var users = new List<User>
        {
            new User { Id = userId1, AvatarUrl="testUrl", Username = "User1" },
            new User { Id = userId2, AvatarUrl="testUrl", Username = "User2" },
            new User { Id = userId3, AvatarUrl="testUrl", Username = "User3" }
        };
        var clan = new Clan { ClanId = Guid.NewGuid(), Name = "Test Clan" , ImagePath="testPath", Description="testDescription"};    
        var clanMemberships = new List<ClanMembership>
        {
            new ClanMembership { ClanId = clanId, UserId =userId1 },
            new ClanMembership { ClanId = clanId, UserId = userId2 },
            new ClanMembership { ClanId = clanId, UserId = userId3 }
        };
        _context.ClanMemberships.AddRange(clanMemberships);
        _context.Users.AddRange(users);
        _context.Clans.Add(clan);

        _context.SaveChanges();

        var result = await _userRepository.GetUsersByClanIdAsync(clanId);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
        Assert.IsTrue(result.Any(u => u.Id == userId1));
        Assert.IsTrue(result.Any(u => u.Id == userId2));
        Assert.IsTrue(result.Any(u => u.Id == userId3));
    }

    [TestMethod]
    public async Task GetUsersByClanIdAsync_ShouldReturnEmptyList_WhenNoUsersInClan()
    {
        var clanId = Guid.NewGuid();
        var clan = new Clan { ClanId = clanId, Name = "Test Clan" , ImagePath="testPath", Description="testDescription"};    
        _context.Clans.Add(clan);
        _context.SaveChanges();

        var result = await _userRepository.GetUsersByClanIdAsync(clanId);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }
    [TestMethod]
    public async Task GetUsersByClanIdAsync_ShouldReturnEmptyList_WhenNoClanFound()
    {
        var clanId = Guid.NewGuid();

        var result = await _userRepository.GetUsersByClanIdAsync(clanId);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}
