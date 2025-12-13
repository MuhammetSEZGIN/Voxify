using System;

namespace MessageService.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }

    public string UsersCollection { get; set; }
    public string MessagesCollection { get; set; }
}
