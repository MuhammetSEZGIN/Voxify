using IdentityService.Models;

public static class TestUsers
{
    public static List<ApplicationUser> GetTestUsers()
    {
        return new List<ApplicationUser>
        {
            new ApplicationUser { Id = "1", UserName = "testuser1", PasswordHash = "Password123!", Email = "testuser1@example.com" },
            new ApplicationUser { Id = "2", UserName = "testuser2", PasswordHash = "Password123!", Email = "testuser2@example.com" },
            new ApplicationUser { Id = "3", UserName = "testuser3", PasswordHash = "Password123!", Email = "testuser3@example.com" }
        };
    }
}