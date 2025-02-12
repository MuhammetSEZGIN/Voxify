using System;
using System.Linq;
using MessageService.Models;

namespace MessageService.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Ensure database is created.
            context.Database.EnsureCreated();

            // If there are already users, do not seed.
            if (context.Users.Any())
                return;

            // Create test users.
            var user1 = new User
            {
                Id = "user1",
                UserName = "Herakles",
                Email = "john@example.com",
                AvatarUrl = "https://www.pngwing.com/en/search?q=user+Avatar",
                Messages = new System.Collections.Generic.List<Message>()
            };

            var user2 = new User
            {
                Id = "user2",
                UserName = "SoldatDesReves",
                Email = "jane@example.com",
                AvatarUrl = "https://www.pngwing.com/en/search?q=user+Avatar",
                Messages = new System.Collections.Generic.List<Message>()
            };

            context.Users.AddRange(user1, user2);
            context.SaveChanges();

            // Create test messages.
            var message1 = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = user1.Id,
                Text = "Hello, this is John!",
                CreatedAt = DateTime.UtcNow,
                ChannelId = Guid.Parse("2B2246C5-212A-484C-9AA0-25630D289CBD"),
                RecipientId = user2.Id
            };

            var message2 = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = user2.Id,
                Text = "Hi John, Jane replying!",
                CreatedAt = DateTime.UtcNow,
                ChannelId = Guid.Parse("2B2246C5-212A-484C-9AA0-25630D289CBD"),
                RecipientId = user1.Id
            };

            context.Messages.AddRange(message1, message2);
            context.SaveChanges();
        }
    }
}