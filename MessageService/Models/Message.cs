using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MessageService.Models;

[BsonIgnoreExtraElements]
public class Message
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    [BsonElement("user_id")]
    public string SenderId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string ClanId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [BsonRepresentation(BsonType.String)]
    public string ChannelId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string RecipientId { get; set; }

}
