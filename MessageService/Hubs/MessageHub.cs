using MessageService.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MessageService.Models;
using System.Security.Claims;

namespace MyProject.MessageService.Hubs
{
    public class MessageHub : Hub
    {
        private readonly ApplicationDbContext _db;

        public MessageHub(ApplicationDbContext db)
        {
            _db = db;
        }

        // Kullanıcı belirtilen channel'a katılır (SignalR Group'a dahil)
        public async Task JoinChannel(int channelId)
        {
            // Token'daki userId
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Kullanıcı kimliği bulunamadı.");

            // Kanalı DB'den çek
            var channel = await _db.Channels
                                   .Include(c => c.Clan)
                                   .FirstOrDefaultAsync(c => c.Id == channelId);

            if (channel == null)
                throw new Exception("Kanal bulunamadı.");

            // Kullanıcı bu klanın üyesi mi?
            bool isMember = await _db.ClanMemberShips
                .AnyAsync(cm => cm.ClanId == channel.ClanId && cm.UserId == userId);
            if (!isMember)
                throw new Exception("Bu kanala girme yetkiniz yok.");

            // SignalR grubu: "channel_1" gibi
            await Groups.AddToGroupAsync(Context.ConnectionId, $"channel_{channelId}");
        }

        // Kanal mesajı gönderme
        public async Task SendChannelMessage(int channelId, string text)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Kullanıcı kimliği bulunamadı.");

            // Kanalı bul
            var channel = await _db.Channels
                                   .Include(c => c.Clan)
                                   .FirstOrDefaultAsync(c => c.Id == channelId);
            if (channel == null)
                throw new Exception("Kanal yok.");

            // Klan üyesi mi kontrol
            bool isMember = await _db.ClanMemberShips
                .AnyAsync(cm => cm.ClanId == channel.ClanId && cm.UserId == userId);
            if (!isMember)
                throw new Exception("Yetkisiz erişim.");

            // Mesajı DB'ye kaydet
            var message = new Message
            {
                SenderId = userId,
                ChannelId = channelId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            // Kanal grubundaki herkese mesajı yolla
            await Clients.Group($"channel_{channelId}")
                .SendAsync("ReceiveChannelMessage", new
                {
                    message.Id,
                    message.SenderId,
                    message.Text,
                    message.CreatedAt,
                    ChannelId = channelId
                });
        }

        // Birebir mesaj
        public async Task SendDirectMessage(string recipientId, string text)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Kullanıcı kimliği bulunamadı.");

            var message = new Message
            {
                SenderId = userId,
                RecipientId = recipientId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            // Sadece alıcıya gönder
            await Clients.User(recipientId)
                .SendAsync("ReceiveDirectMessage", new
                {
                    message.Id,
                    message.SenderId,
                    message.RecipientId,
                    message.Text,
                    message.CreatedAt
                });

            // İsteğe bağlı: Gönderene de göster
            await Clients.User(userId)
                .SendAsync("ReceiveDirectMessage", new
                {
                    message.Id,
                    message.SenderId,
                    message.RecipientId,
                    message.Text,
                    message.CreatedAt
                });
        }
    }
}
