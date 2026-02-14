using IdentityService.Models;

public static class MockData
{
    public static List<ApplicationUser> GetMockUsers()
    {
        return new List<ApplicationUser>
        {
            new ApplicationUser { Id = "1", UserName = "testuser1", PasswordHash = "Password123", Email = "test1@example.com" },
            new ApplicationUser { Id = "2", UserName = "testuser2", PasswordHash = "Password123", Email = "test2@example.com" }
        };
    }

    public static ApplicationUser GetMockUser()
    {
        return new ApplicationUser { Id = "1", UserName = "testuser1", PasswordHash = "Password123", Email = "test1@example.com" };
    }
}