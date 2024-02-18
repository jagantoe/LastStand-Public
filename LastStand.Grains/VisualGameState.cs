using LastStand.Game;
using System.Text.Json;

namespace LastStand.Grains;

/// <summary>
/// State for the dashboard
/// </summary>
public class VisualGameState
{
    public VisualGameState() { }
    public VisualGameState(Game.Game game)
    {
        Name = game.Name;
        RoundHighScore = game.RoundHighScore;
        Round = game.Round;
        AttackersKilled = game.AttackersKilled;
        Timer = game.Timer;
        Status = game.Status;
        GameActive = game.GameActive;
        BaseHealth = game.BaseHealth;
        Resources = game.Resources;
        Inventory = game.Inventory;
        Map = new MapState(game.GameMap);
        Players = game.Players.Select(x => new VisualPlayer(x));
        Attackers = game.Attackers.Select(x => new VisualCharacter(x));
        StaticCharacters = game.StaticCharacters.Select(x => new VisualCharacter(x));
    }
    public string Name { get; set; }
    public uint RoundHighScore { get; set; }
    public uint Round { get; set; }
    public uint AttackersKilled { get; set; }
    public uint Timer { get; set; }
    public string Status { get; set; }
    public bool GameActive { get; set; }
    public uint BaseHealth { get; set; }
    public Resources Resources { get; set; }
    public Dictionary<string, uint> Inventory { get; set; }
    public MapState Map { get; set; }
    public IEnumerable<VisualPlayer> Players { get; set; }
    public IEnumerable<VisualCharacter> Attackers { get; set; }
    public IEnumerable<VisualCharacter> StaticCharacters { get; set; }
}

public class MapState
{
    public MapState() { }
    public MapState(Map map)
    {
        MapSize = map.MapSize;
        Tiles = map.MapTiles.Values.Select(x => new TileState(x)).ToList();
    }
    public int MapSize { get; set; }
    public List<TileState> Tiles { get; set; }
}
public class TileState
{
    public TileState() { }
    public TileState(Tile tile)
    {
        Pos = tile.Pos.Serialize();
        Building = tile.Building;
        Resource = tile.Resource switch
        {
            null => "grass",
            SmallGrain or MediumGrain => "grain",
            LargeGrain => "grain_full",
            Tree => "wood",
            Forest => "wood_full",
            StonePile => "stone",
            SteelMine => "steel",
            _ => "grass"
        };
    }
    public string Pos { get; set; }
    public string? Building { get; set; }
    public string Resource { get; set; }
}

public class VisualCharacter
{
    public VisualCharacter() { }
    public VisualCharacter(Character character)
    {
        Name = character.Name;
        Boy = character.Boy;
        Pos = character.Pos.Serialize();
        CurrentHealth = character.CurrentHealth;
        MaxHealth = character.MaxHealth;
        BaseStats = character.BaseStats;
        Dead = character.Dead;
        CombatBehaviour = character.CombatBehaviour;
        Commands = character.Commands.Select(x => JsonSerializer.Serialize(x));
        TotalDamageDone = character.TotalDamageDone;
    }
    public string Name { get; set; }
    public bool Boy { get; set; }
    public string Pos { get; set; }
    public uint CurrentHealth { get; set; }
    public uint MaxHealth { get; set; }
    public Stats BaseStats { get; set; }
    public bool Dead { get; set; }
    public CombatBehaviour CombatBehaviour { get; set; }
    public IEnumerable<string> Commands { get; set; }
    public uint TotalDamageDone { get; set; }
}

public class VisualPlayer : VisualCharacter
{
    public VisualPlayer() : base() { }
    public VisualPlayer(PlayerCharacter player) : base(player)
    {
        Active = player.Active;
        DerivedStats = player.DerivedStats;
        EquipedItems = player.EquipedItems.ToDictionary(x => x.Key, x => x.Value.Name);
        Bag = player.Bag;
        Deaths = player.Deaths;
    }
    public bool Active { get; set; }
    public Stats DerivedStats { get; set; }
    public Dictionary<ItemCategory, string> EquipedItems { get; set; }
    public Resources Bag { get; set; }
    public int Deaths { get; set; }
}