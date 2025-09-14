using System;

namespace IdentityService.Models;

public class LogAction
{
    public Guid Id { get; set; }
    public string ActionName { get; set; }
    public string UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Details { get; set; }

    public LogAction()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }
}
