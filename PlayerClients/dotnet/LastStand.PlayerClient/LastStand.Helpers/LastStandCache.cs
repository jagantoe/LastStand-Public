using StackExchange.Redis;
using System.Text.Json;

namespace LastStand.Helpers;
public class LastStandCache
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    public readonly IDatabase _database;
    public readonly ISubscriber _subscriber;

    public LastStandCache()
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(Setup.CacheKey);
        _database = _connectionMultiplexer.GetDatabase();
        _subscriber = _connectionMultiplexer.GetSubscriber();
    }

    public Game GetGame()
    {
        var state = _database.StringGet(Setup.User);
        return JsonSerializer.Deserialize<Game>(state);
    }
}
