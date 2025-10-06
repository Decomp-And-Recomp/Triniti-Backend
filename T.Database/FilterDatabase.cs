namespace T.Database;

public abstract class FilterDatabase
{
    /// <summary>
    /// Filters the <paramref name="value"/>
    /// </summary>
    /// <returns>A filtered version of <paramref name="value"/></returns>
    public abstract Task<string?> Filter(string? value);
}
