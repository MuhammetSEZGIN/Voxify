using ClanService.Data;
using Microsoft.EntityFrameworkCore;
using ClanService.Models;

namespace ClanService.Repositories.Tests;

[TestClass]
public class VoiceChannelRepositoryTest
{
    private VoiceChannelRepository _voiceChannelRepository;
    private ApplicationDbContext _context;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _voiceChannelRepository = new VoiceChannelRepository(_context);
    }

    [TestMethod]
    public async Task GetVoiceChannelsByClanIdAsync_ShouldReturnVoiceChannels_WhenChannelsExist()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var voiceChannel1 = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Test Voice Channel 1",
            ClanId = clanId,
            IsActive = true,
            MaxParticipants = 5
        };
        var voiceChannel2 = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Test Voice Channel 2",
            ClanId = clanId,
            IsActive = true,
            MaxParticipants = 10
        };
        
        _context.VoiceChannels.AddRange(voiceChannel1, voiceChannel2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _voiceChannelRepository.GetVoiceChannelsByClanIdAsync(clanId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(c => c.VoiceChannelId == voiceChannel1.VoiceChannelId));
        Assert.IsTrue(result.Any(c => c.VoiceChannelId == voiceChannel2.VoiceChannelId));
    }

    [TestMethod]
    public async Task GetVoiceChannelsByClanIdAsync_ShouldReturnEmptyList_WhenNoChannelsExist()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        // Act
        var result = await _voiceChannelRepository.GetVoiceChannelsByClanIdAsync(clanId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetVoiceChannelsByClanIdAsync_ShouldReturnOnlyChannelsForSpecificClan()
    {
        // Arrange
        var clanId1 = Guid.NewGuid();
        var clanId2 = Guid.NewGuid();
        
        var voiceChannel1 = new VoiceChannel 
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Voice Channel 1", 
            ClanId = clanId1,
            IsActive = true,
            MaxParticipants = 5
        };
        
        var voiceChannel2 = new VoiceChannel 
        { 
            VoiceChannelId = Guid.NewGuid(),
            Name = "Voice Channel 2", 
            ClanId = clanId1,
            IsActive = true,
            MaxParticipants = 5
        };
        
        var voiceChannel3 = new VoiceChannel 
        { 
            VoiceChannelId = Guid.NewGuid(),
            Name = "Voice Channel 3", 
            ClanId = clanId2,
            IsActive = true,
            MaxParticipants = 5
        };
        
        _context.VoiceChannels.AddRange(voiceChannel1, voiceChannel2, voiceChannel3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _voiceChannelRepository.GetVoiceChannelsByClanIdAsync(clanId1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(c => c.ClanId == clanId1));
        Assert.IsFalse(result.Any(c => c.ClanId == clanId2));
    }

    [TestMethod]
    public async Task AddAsync_ShouldAddVoiceChannel()
    {
        // Arrange
        var voiceChannel = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Test Voice Channel",
            ClanId = Guid.NewGuid(),
            IsActive = true,
            MaxParticipants = 8
        };

        // Act
        await _voiceChannelRepository.AddAsync(voiceChannel);

        // Assert
        var result = await _context.VoiceChannels.FindAsync(voiceChannel.VoiceChannelId);
        Assert.IsNotNull(result);
        Assert.AreEqual(voiceChannel.Name, result.Name);
        Assert.AreEqual(voiceChannel.ClanId, result.ClanId);
        Assert.AreEqual(voiceChannel.IsActive, result.IsActive);
        Assert.AreEqual(voiceChannel.MaxParticipants, result.MaxParticipants);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnVoiceChannel_WhenChannelExists()
    {
        // Arrange
        var voiceChannel = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Test Voice Channel",
            ClanId = Guid.NewGuid(),
            IsActive = true,
            MaxParticipants = 8
        };
        
        _context.VoiceChannels.Add(voiceChannel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _voiceChannelRepository.GetByIdAsync(voiceChannel.VoiceChannelId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(voiceChannel.VoiceChannelId, result.VoiceChannelId);
        Assert.AreEqual(voiceChannel.Name, result.Name);
        Assert.AreEqual(voiceChannel.ClanId, result.ClanId);
    }
    
    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenChannelDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _voiceChannelRepository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateVoiceChannel()
    {
        // Arrange
        var voiceChannel = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Original Name",
            ClanId = Guid.NewGuid(),
            IsActive = true,
            MaxParticipants = 5
        };
        
        _context.VoiceChannels.Add(voiceChannel);
        await _context.SaveChangesAsync();
        
        // Detach the entity to simulate realistic scenario
        _context.Entry(voiceChannel).State = EntityState.Detached;
        
        // Create modified version
        var modifiedVoiceChannel = new VoiceChannel
        {
            VoiceChannelId = voiceChannel.VoiceChannelId,
            Name = "Updated Name",
            ClanId = voiceChannel.ClanId,
            IsActive = false,
            MaxParticipants = 10
        };

        // Act
        await _voiceChannelRepository.UpdateAsync(modifiedVoiceChannel);

        // Assert
        var result = await _context.VoiceChannels.FindAsync(voiceChannel.VoiceChannelId);
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Name", result.Name);
        Assert.AreEqual(false, result.IsActive);
        Assert.AreEqual(10, result.MaxParticipants);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldDeleteVoiceChannel()
    {
        // Arrange
        var voiceChannel = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Test Voice Channel",
            ClanId = Guid.NewGuid(),
            IsActive = true,
            MaxParticipants = 5
        };
        
        _context.VoiceChannels.Add(voiceChannel);
        await _context.SaveChangesAsync();

        // Act
        await _voiceChannelRepository.DeleteAsync(voiceChannel);

        // Assert
        var result = await _context.VoiceChannels.FindAsync(voiceChannel.VoiceChannelId);
        Assert.IsNull(result);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}