public static class TestUsers
{
    public static List<User> GetTestUsers()
    {
        return new List<User>
        {
            new User { Id = 1, Username = "testuser1", Password = "Password123!", Email = "testuser1@example.com" },
            new User { Id = 2, Username = "testuser2", Password = "Password123!", Email = "testuser2@example.com" },
            new User { Id = 3, Username = "testuser3", Password = "Password123!", Email = "testuser3@example.com" }
        };
    }
}