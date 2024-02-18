using System.Text.Json;

namespace LastStand.Grains;

public class GameContainer
{
    [NonSerialized]
    public Game.Game game;

    // Always store the serialized state and deserialize it via code to avoid Orleans serializer
    public string GameJSON
    {
        get { return JsonSerializer.Serialize(game); }
        set { game = JsonSerializer.Deserialize<Game.Game>(value); }
    }
}