namespace LastStand.Grains;

public interface IGrainCollector : IGrainWithStringKey
{
    Task<List<string>> Get();
    Task Register(string identifier);
    Task Deregister(string identifier);
}

[KeepAlive]
public sealed class GrainCollectorGrain : Grain, IGrainCollector
{
    public static string Name = "list";
    private readonly List<string> Grains = [];

    public Task<List<string>> Get()
    {
        return Task.FromResult(Grains);
    }

    public Task Register(string identifier)
    {
        Grains.Add(identifier);
        return Task.CompletedTask;
    }

    public Task Deregister(string identifier)
    {
        Grains.Remove(identifier);
        return Task.CompletedTask;
    }
}