public class DatabaseFixture : IDisposable
{
    public DatabaseContext Context { get; private set; }

    public DatabaseFixture()
    {
        Context = new DatabaseContext();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}