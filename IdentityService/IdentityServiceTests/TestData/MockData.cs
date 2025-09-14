public static class MockData
{
    public static List<User> GetMockUsers()
    {
        return new List<User>
        {
            new User { Id = 1, Username = "testuser1", Password = "Password123", Email = "test1@example.com" },
            new User { Id = 2, Username = "testuser2", Password = "Password123", Email = "test2@example.com" }
        };
    }

    public static User GetMockUser()
    {
        return new User { Id = 1, Username = "testuser1", Password = "Password123", Email = "test1@example.com" };
    }
}