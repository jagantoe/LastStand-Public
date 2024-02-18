namespace LastStand.Helpers;
public class Game
{
    public string Name { get; set; }
    public int RoundHighScore { get; set; }
    public int Round { get; set; }
    public int Timer { get; set; }
    public string Status { get; set; }
    public bool GameActive { get; set; }
    public int BaseHealth { get; set; }
    public Resources Resources { get; set; }
    public Dictionary<string, int> Buildings { get; set; }
    public Dictionary<string, int> Inventory { get; set; }
    public List<Player> Players { get; set; }
    public List<Character> Attackers { get; set; }
    public List<Character> StaticCharacters { get; set; }
}

public class Character
{
    public string Name { get; set; }
    public string Pos { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public Stats BaseStats { get; set; }
    public bool Dead { get; set; }
    public CombatBehaviour CombatBehaviour { get; set; }
    public List<Command> Commands { get; set; }
    public int TotalDamageDone { get; set; }
    public int DistanceToBase { get; set; }
}

public class Player : Character
{
    public bool Active { get; set; }
    public bool IsHome { get; set; }
    public int MaxCommands { get; set; }
    public Stats DerivedStats { get; set; }
    public Dictionary<string, string> EquipedItems { get; set; }
    public Resources Bag { get; set; }
    public int Deaths { get; set; }
}

public class Resources
{
    public int Grain { get; set; }
    public int Wood { get; set; }
    public int Stone { get; set; }
    public int Steel { get; set; }
    public int Limit { get; set; }
}

public class Stats
{
    public int Strength { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }

    public int Damage { get; set; }
    public int AttackSpeed { get; set; }
    public int AttackRange { get; set; }
    public int Pierce { get; set; }
    public int Block { get; set; }
    public int DamageTakenModifier { get; set; }

    public int GrainModifier { get; set; }
    public int WoodModifier { get; set; }
    public int StoneModifier { get; set; }
    public int SteelModifier { get; set; }

    public int CarryModifier { get; set; }
}

public class Command
{
    public string Name { get; set; }
    public string? Target { get; set; }
    public int Timer { get; set; }
}