using System.Threading.Tasks;
using ClanService.Data;
using ClanService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClanService.Repositories.Tests;

[TestClass]
public class ClanRepositoryTest
{
    private ApplicationDbContext _context;
    private ClanRepository _clanRepository;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _clanRepository = new ClanRepository(_context);
    }
    [TestMethod]

    public async Task GetClansByUserIdAsync_ShouldReturnClans_WhenUserIdIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        var clanId1 = Guid.NewGuid();
        var clanId2 = Guid.NewGuid();

        var clan1 = new Clan { ClanId = clanId1, Name = "Clan 1", Description = "Description 1" };
        var clan2 = new Clan { ClanId = clanId2, Name = "Clan 2", Description = "Description 2" };

        var ClanMembership1 = new ClanMembership { UserId = userId, ClanId = clanId1 };
        var ClanMembership2 = new ClanMembership { UserId = userId, ClanId = clanId2 };

        _context.ClanMemberships.AddRange(ClanMembership1, ClanMembership2);
        _context.Clans.AddRange(clan1, clan2);

        _context.SaveChanges();

        // Act
        var result = await _clanRepository.GetClansByUserIdAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());

        Assert.IsTrue(result.Any(c => c.ClanId == clanId1));
        Assert.IsTrue(result.Any(c => c.ClanId == clanId2));


        Assert.IsTrue(result.Any(c => c.ClanId == ClanMembership1.ClanId));
    }

    [TestMethod]
    public async Task GetClanWithDetailsAsync_ShouldReturnClan_WhenClanIdIsValid()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var clan = new Clan { ClanId = clanId, Name = "Clan 1", Description = "Description 1" };
        _context.Clans.Add(clan);
        _context.SaveChanges();

        // Act
        var result = await _clanRepository.GetClanWithDetailsAsync(clanId);

        // Assert
        Assert.IsNotNull(result);

        Assert.AreEqual(clanId, result.ClanId);
        Assert.AreEqual("Clan 1", result.Name);
        Assert.AreEqual("Description 1", result.Description);
        Assert.IsNotNull(result.Channels);
        Assert.IsNotNull(result.VoiceChannels);
        Assert.IsNotNull(result.ClanMemberShips);
        Assert.IsNotNull(result.ClanInvitations);
    }

    [TestMethod]
    public async Task SearchClansAsync_ShouldReturnMatchingClans_WhenSearchingByName()
    {
        // Arrange
        var clan1 = new Clan { ClanId = Guid.NewGuid(), Name = "Gaming Clan", Description = "Regular clan", ImagePath = "path1" };
        var clan2 = new Clan { ClanId = Guid.NewGuid(), Name = "Coding Team", Description = "For developers", ImagePath = "path2" };
        var clan3 = new Clan { ClanId = Guid.NewGuid(), Name = "Book Club", Description = "Gaming discussions", ImagePath = "path3" };

        _context.Clans.AddRange(clan1, clan2, clan3);
        await _context.SaveChangesAsync();

        string searchText = "gaming";
        int limit = 2;
        int page = 1;
        
        // Act
        var result = await _clanRepository.SearchClansAsync(searchText, limit, page);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(c => c.ClanId == clan1.ClanId)); // Matches name
        Assert.IsFalse(result.Any(c => c.ClanId == clan2.ClanId)); // Doesn't match
        Assert.IsTrue(result.Any(c => c.ClanId == clan3.ClanId)); // Matches description
    }

    [TestMethod]
    public async Task SearchClansAsync_ShouldReturnMatchingClans_WhenSearchingByDescription()
    {
        // Arrange
        var clan1 = new Clan { ClanId = Guid.NewGuid(), Name = "Normal Clan", Description = "Focused on RPG gaming", ImagePath = "path1" };
        var clan2 = new Clan { ClanId = Guid.NewGuid(), Name = "Normal Team", Description = "For developers", ImagePath = "path2" };

        _context.Clans.AddRange(clan1, clan2);
        await _context.SaveChangesAsync();

        string searchText = "RPG";
        int limit = 10;
        int page = 1;
        
        // Act
        var result = await _clanRepository.SearchClansAsync(searchText, limit, page);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Any(c => c.ClanId == clan1.ClanId));
        Assert.IsFalse(result.Any(c => c.ClanId == clan2.ClanId));
    }

    [TestMethod]
    public async Task SearchClansAsync_ShouldPaginateCorrectly()
    {
        // Arrange
        var clans = new List<Clan>();
        for (int i = 0; i < 10; i++)
        {
            clans.Add(new Clan 
            { 
                ClanId = Guid.NewGuid(),
                Name = $"Test Clan {i}",
                Description = "Same description",
                ImagePath = $"path{i}"
            });
        }
        
        _context.Clans.AddRange(clans);
        await _context.SaveChangesAsync();

        string searchText = "Test Clan";
        int limit = 3;
        
        // Act
        var page1Result = await _clanRepository.SearchClansAsync(searchText, limit, 1);
        var page2Result = await _clanRepository.SearchClansAsync(searchText, limit, 2);
        var page3Result = await _clanRepository.SearchClansAsync(searchText, limit, 3);
        var page4Result = await _clanRepository.SearchClansAsync(searchText, limit, 4);
        var page5Result = await _clanRepository.SearchClansAsync(searchText, limit, 5);

        // Assert
        Assert.AreEqual(3, page1Result.Count());
        Assert.AreEqual(3, page2Result.Count());
        Assert.AreEqual(3, page3Result.Count());
        Assert.AreEqual(1, page4Result.Count());
        Assert.AreEqual(0, page5Result.Count());
    }

    [TestMethod]
    public async Task SearchClansAsync_ShouldReturnAllClans_WhenSearchTextIsEmpty()
    {
        // Arrange
        var clan1 = new Clan { ClanId = Guid.NewGuid(), Name = "Clan 1", Description = "Description 1", ImagePath = "path1" };
        var clan2 = new Clan { ClanId = Guid.NewGuid(), Name = "Clan 2", Description = "Description 2", ImagePath = "path2" };
        
        _context.Clans.AddRange(clan1, clan2);
        await _context.SaveChangesAsync();

        string searchText = "";
        int limit = 10;
        int page = 1;
        
        // Act
        var result = await _clanRepository.SearchClansAsync(searchText, limit, page);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

      [TestMethod]
    public async Task GetClansByUserIdAsync_ShouldReturnEmptyList_WhenUserHasNoClans()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await _clanRepository.GetClansByUserIdAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

        [TestMethod]
    public async Task GetClanWithDetailsAsync_ShouldReturnNull_WhenClanDoesNotExist()
    {
        // Arrange
        var nonExistentClanId = Guid.NewGuid();

        // Act
        var result = await _clanRepository.GetClanWithDetailsAsync(nonExistentClanId);

        // Assert
        Assert.IsNull(result);
    }

     [TestMethod]
    public async Task AddAsync_ShouldAddClan()
    {
        // Arrange
        var clan = new Clan
        {
            ClanId = Guid.NewGuid(),
            Name = "New Clan",
            Description = "New Description",
            ImagePath = "new/path"
        };

        // Act
        await _clanRepository.AddAsync(clan);

        // Assert
        var result = await _context.Clans.FindAsync(clan.ClanId);
        Assert.IsNotNull(result);
        Assert.AreEqual(clan.Name, result.Name);
        Assert.AreEqual(clan.Description, result.Description);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateClan()
    {
        // Arrange
        var clan = new Clan
        {
            ClanId = Guid.NewGuid(),
            Name = "Original Name",
            Description = "Original Description",
            ImagePath = "original/path"
        };
        
        _context.Clans.Add(clan);
        await _context.SaveChangesAsync();
        
        // Detach the entity to simulate a real-world scenario
        _context.Entry(clan).State = EntityState.Detached;

        // Modify the clan
        var modifiedClan = new Clan
        {
            ClanId = clan.ClanId,
            Name = "Updated Name",
            Description = "Updated Description",
            ImagePath = "updated/path"
        };

        // Act
        await _clanRepository.UpdateAsync(modifiedClan);

        // Assert
        var result = await _context.Clans.FindAsync(clan.ClanId);
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Name", result.Name);
        Assert.AreEqual("Updated Description", result.Description);
        Assert.AreEqual("updated/path", result.ImagePath);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldDeleteClan()
    {
        // Arrange
        var clan = new Clan
        {
            ClanId = Guid.NewGuid(),
            Name = "Test Clan",
            Description = "Test Description",
            ImagePath = "test/path"
        };
        
        _context.Clans.Add(clan);
        await _context.SaveChangesAsync();

        // Act
        await _clanRepository.DeleteAsync(clan);

        // Assert
        var result = await _context.Clans.FindAsync(clan.ClanId);
        Assert.IsNull(result);
    }


    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}
