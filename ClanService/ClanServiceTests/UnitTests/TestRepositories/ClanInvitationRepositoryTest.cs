using ClanService.Data;
using ClanService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClanService.Repositories.Tests;

[TestClass]
public class ClanInvitationRepositoryTest
{
    private ClanInvitationRepository _clanInvitationRepository;
    private ApplicationDbContext _context ;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options); 
        _clanInvitationRepository = new ClanInvitationRepository(_context);
    }

    [TestMethod]
    public void GetByCodeAsync_ShouldReturnClanInvitation_WhenCodeExists(){
        var clanInvitation = new ClanInvitation
        {
            InviteId = Guid.NewGuid(),
            InviteCode = "TestCode",
            ClanId = Guid.NewGuid(),
            IsActive = true,
            ExpiresAt = DateTime.UtcNow,
            MaxUses = 5,
            UsedCount = 0,
            Clan = new Clan
            {
                ClanId = Guid.NewGuid(),
                Name = "Test Clan",
                Description = "Test Description",
                ImagePath = "TestPath", 
                ClanInvitations = new List<ClanInvitation>()
            }
        };

        _context.ClanInvitations.Add(clanInvitation);
        _context.SaveChanges();
        var result = _clanInvitationRepository.GetByCodeAsync("TestCode").Result;
        Assert.IsNotNull(result);
        Assert.AreEqual(clanInvitation, result);

    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}
