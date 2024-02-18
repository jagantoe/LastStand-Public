using LastStand.Game;
using System.Text.Json;

namespace LastStand.Grains;

/// <summary>
/// State for the players
/// </summary>
public class PlayGameState(Game.Game game)
{
    public string Name { get; set; } = game.Name;
    public uint RoundHighScore { get; set; } = game.RoundHighScore;
    public uint Round { get; set; } = game.Round;
    public uint Timer { get; set; } = game.Timer;
    public string Status { get; set; } = game.Status;
    public bool GameActive { get; set; } = game.GameActive;
    public uint BaseHealth { get; set; } = game.BaseHealth;
    public Resources Resources { get; set; } = game.Resources;
    public Dictionary<string, uint> Buildings { get; set; } = game.Buildings;
    public Dictionary<string, uint> Inventory { get; set; } = game.Inventory;
    public IEnumerable<PlayPlayer> Players { get; set; } = game.Players.Select(x => new PlayPlayer(x));
    public IEnumerable<PlayCharacter> Attackers { get; set; } = game.Attackers.Select(x => new PlayCharacter(x));
    public IEnumerable<PlayCharacter> StaticCharacters { get; set; } = game.StaticCharacters.Select(x => new PlayCharacter(x));
}

public class PlayCharacter(Character character)
{
    public string Name { get; set; } = character.Name;
    public string Pos { get; set; } = character.Pos.Serialize();
    public uint MaxHealth { get; set; } = character.MaxHealth;
    public uint CurrentHealth { get; set; } = character.CurrentHealth;
    public Stats BaseStats { get; set; } = character.BaseStats;
    public bool Dead { get; set; } = character.Dead;
    public CombatBehaviour CombatBehaviour { get; set; } = character.CombatBehaviour;
    public IEnumerable<PlayCommand> Commands { get; set; } = character.Commands.Select(x => new PlayCommand(x));
    public uint TotalDamageDone { get; set; } = character.TotalDamageDone;
    public int DistanceToBase { get; set; } = character.Pos.Distance(Constants.BasePosition);
}

public class PlayPlayer(PlayerCharacter player) : PlayCharacter(player)
{
    public bool Active { get; set; } = player.Active;
    public bool IsHome { get; set; } = player.IsHome;
    public int MaxCommands { get; set; } = player.MaxCommands;
    public Stats DerivedStats { get; set; } = player.DerivedStats;
    public Dictionary<string, string> EquipedItems { get; set; } = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(player.EquipedItems));
    public Resources Bag { get; set; } = player.Bag;
    public int Deaths { get; set; } = player.Deaths;
}

public class PlayCommand(Command command)
{
    public string Name { get; set; } = command.Name;
    public string? Target { get; set; } = command.Target;
    public int Timer { get; set; } = command.Timer;
}