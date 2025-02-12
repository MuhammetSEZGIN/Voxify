using System;
using System.ComponentModel.DataAnnotations;

namespace MessageService.Models;

public class User
{
    [Key]
    public string Id { get; set; }
    public string UserName { get; set; } 
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public List<Message> Messages { get; set; }

}
