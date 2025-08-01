namespace T.TNet;

public static class TNetInstanceManager
{
    public static List<TNetInstance> instances = new();

    public static TNetInstance CreateInstance(int port, int gameId)
    {
        TNetInstance result = new(port, gameId);

        instances.Add(result);

        return result;
    }
}
