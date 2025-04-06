using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ClanService.Data;
using ClanService.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace ClanServiceTests.UnitTests.Repositories;
[TestClass]
public class ClanRepository
{
    private readonly string dbName = "TestDatabase";
    
    private DbContextOptions<ApplicationDbContext> GetInMemoryDbOptions(){
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
    }

    [TestMethod]
    public async Task GetbyIdAsync_ShouldReturnClan_WhenClanExists()
    {
        var options = GetInMemoryDbOptions();   
        var clanId = Guid.NewGuid();

        using (var context = new ApplicationDbContext(options))
        {
            context.Clans.Add(new Clan
            {
                ClanId = clanId,
                Name = "Test Clan",
                ImagePath = "test.png",
                Description = "Test Description",
            });
            context.SaveChanges();  
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new ClanService.Repositories.ClanRepository(context);
            var clan = await repository.GetByIdAsync(clanId);

            Assert.IsNotNull(clan);
            Assert.AreEqual(clanId, clan.ClanId);
            Assert.AreEqual("Test Clan", clan.Name);
        }
        
        
    }
        
}

