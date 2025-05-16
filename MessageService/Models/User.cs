using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessageService.Models;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    public string Id { get; set; }
    public string UserName { get; set; } 
    public string AvatarUrl { get; set; }

}
