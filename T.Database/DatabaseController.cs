namespace T.Database;

public abstract class DatabaseController
{
    public BanDatabase banDatabase { get; protected set; } = null!;
    public FilterDatabase filterDatabase { get; protected set; } = null!;
    public DinoHunterDatabase dinoHunterDatabase { get; protected set; } = null!;


    public readonly DatabaseLogger logger = new();

    /// <summary>
    /// Called on the app launch, made for database to initialize all of its tables and etc.
    /// </summary>
    public abstract Task Initialize(string server, int port, string database, string user, string password);
}
