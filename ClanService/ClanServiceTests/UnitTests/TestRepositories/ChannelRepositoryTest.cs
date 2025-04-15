using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClanService.Data;
using ClanService.Repositories;
using Microsoft.EntityFrameworkCore;
using ClanService.Models;
namespace ClanService.Repositories.Tests;

[TestClass]
public class ChannelRepositoryTest
{
    private ChannelRepository _channelRepository;
    private ApplicationDbContext _context;



    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
        _context.Database.EnsureDeleted();
        _channelRepository = new ChannelRepository(_context);
    }

    [TestMethod]
    public async Task GetChannelsByClanIdAsync_ShouldReturnChannels_WhenChannelsExist()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var channel1 = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Test Channel 1",
            ClanId = clanId,
        };
        var channel2 = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Test Channel 2",
            ClanId = clanId,
        };
        _context.Channels.Add(channel1);
        _context.Channels.Add(channel2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _channelRepository.GetChannelsByClanIdAsync(clanId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(c => c.ChannelId == channel1.ChannelId));
        Assert.IsTrue(result.Any(c => c.ChannelId == channel2.ChannelId));
    }

    [TestMethod]
    public async Task GetChannelsByClanIdAsync_ShouldReturnEmptyList_WhenNoChannelsExist()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        // Act
        var result = await _channelRepository.GetChannelsByClanIdAsync(clanId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetChannelsByClanIdAsync_ShouldReturnOnlyChannelsForSpecificClan()
    {
        // Arrange
        var clanId1 = Guid.NewGuid();
        var clanId2 = Guid.NewGuid();
        
        var channel1 = new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 1", ClanId = clanId1 };
        var channel2 = new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 2", ClanId = clanId1 };
        var channel3 = new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 3", ClanId = clanId2 };
        
        _context.Channels.AddRange(channel1, channel2, channel3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _channelRepository.GetChannelsByClanIdAsync(clanId1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(c => c.ClanId == clanId1));
        Assert.IsFalse(result.Any(c => c.ClanId == clanId2));
    }

    [TestMethod]
    public async Task DeleteChannelsByClanIdAsync_ShouldDeleteChannels_WhenChannelsExist()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var channel1 = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Test Channel 1",
            ClanId = clanId,
        };
        var channel2 = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Test Channel 2",
            ClanId = clanId,
        };
        _context.Channels.AddRange(channel1, channel2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _channelRepository.DeleteChannelsByClanIdAsync(clanId);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(0, _context.Channels.Count(c => c.ClanId == clanId));
    }

    [TestMethod]
    public async Task DeleteChannelsByClanIdAsync_ShouldReturnFalse_WhenNoChannelsExist()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        // Act
        var result = await _channelRepository.DeleteChannelsByClanIdAsync(clanId);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task DeleteChannelsByClanIdAsync_ShouldOnlyDeleteChannelsForSpecificClan()
    {
        // Arrange
        var clanId1 = Guid.NewGuid();
        var clanId2 = Guid.NewGuid();
        
        var channel1 = new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 1", ClanId = clanId1 };
        var channel2 = new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 2", ClanId = clanId1 };
        var channel3 = new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 3", ClanId = clanId2 };
        
        _context.Channels.AddRange(channel1, channel2, channel3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _channelRepository.DeleteChannelsByClanIdAsync(clanId1);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(0, _context.Channels.Count(c => c.ClanId == clanId1));
        Assert.AreEqual(1, _context.Channels.Count(c => c.ClanId == clanId2));
    }

    [TestMethod]
    public async Task AddAsync_ShouldAddChannel()
    {
        // Arrange
        var channel = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Test Channel",
            ClanId = Guid.NewGuid()
        };

        // Act
        await _channelRepository.AddAsync(channel);

        // Assert
        var result = await _context.Channels.FindAsync(channel.ChannelId);
        Assert.IsNotNull(result);
        Assert.AreEqual(channel.Name, result.Name);
        Assert.AreEqual(channel.ClanId, result.ClanId);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnChannel_WhenChannelExists()
    {
        // Arrange
        var channel = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Test Channel",
            ClanId = Guid.NewGuid()
        };
        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _channelRepository.GetByIdAsync(channel.ChannelId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(channel.ChannelId, result.ChannelId);
        Assert.AreEqual(channel.Name, result.Name);
        Assert.AreEqual(channel.ClanId, result.ClanId);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenChannelDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _channelRepository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateChannel()
    {
        // Arrange
        var channel = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Original Name",
            ClanId = Guid.NewGuid()
        };
        
        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();
        
        // Detach the entity to simulate a real-world scenario
        _context.Entry(channel).State = EntityState.Detached;

        // Modify the channel
        var modifiedChannel = new Channel
        {
            ChannelId = channel.ChannelId,
            Name = "Updated Name",
            ClanId = channel.ClanId
        };

        // Act
        await _channelRepository.UpdateAsync(modifiedChannel);

        // Assert
        var result = await _context.Channels.FindAsync(channel.ChannelId);
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Name", result.Name);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldDeleteChannel()
    {
        // Arrange
        var channel = new Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Test Channel",
            ClanId = Guid.NewGuid()
        };
        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();

        // Act
        await _channelRepository.DeleteAsync(channel);

        // Assert
        var result = await _context.Channels.FindAsync(channel.ChannelId);
        Assert.IsNull(result);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}
