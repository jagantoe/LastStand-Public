using System.Numerics;
using System.Text.Json.Serialization;
using static LastStand.Game.Extensions;

namespace LastStand.Game;
public class PlayerCharacter : Character
{
    public bool Active { get; set; } = false;

    [JsonConverter(typeof(VectorJsonConverter))]
    public Vector3 Home { get; set; }
    public bool IsHome => Home == Pos;

    public required int MaxCommands { get; set; }
    public Stats DerivedStats { get; set; }
    public override Stats GetStats() => DerivedStats;

    public Dictionary<ItemCategory, Item> EquipedItems { get; set; } = new();
    public Resources Bag { get; set; } = new() { Limit = 50 };

    public ushort Deaths { get; set; }

    public void CalculateDerivedStats()
    {
        DerivedStats = BaseStats + EquipedItems.Values;
        Bag.Limit = BaseStats.Strength * 2 * (1 + DerivedStats.CarryModifier);
    }

    public void SendOut()
    {
        CalculateDerivedStats();
        Active = true;
    }

    // When the player returns home
    public void BackHome()
    {
        Bag.Clear();
    }

    // When the player has no more commands and becomes unactive
    public void Sleep()
    {
        Active = false;
    }

    // Heal 20% Health
    public void Heal()
    {
        // Heal health
        var heal = (uint)Math.Floor(MaxHealth / 5d);
        CurrentHealth += heal;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
    }

    public void Die()
    {
        Deaths++; // Increment death count
        Pos = Home; // Return home
        Bag.Clear(); // Clear any collected resources
        var stasisTime = 5 * Deaths; // Linear rising death time
        Commands.Clear(); // Clear all commands
        Commands.AddFirst(new StasisCommand(stasisTime)); // Set Stasis
    }

    public void Revive()
    {
        // Set to half health
        CurrentHealth = MaxHealth / 2;
    }

    public void PickUpResource(ResourceType type, uint amount)
    {
        var stats = GetStats();
        switch (type)
        {
            case ResourceType.Grain:
                amount *= 1u + stats.GrainModifier;
                break;
            case ResourceType.Wood:
                amount *= 1u + stats.WoodModifier;
                break;
            case ResourceType.Stone:
                amount *= 1u + stats.StoneModifier;
                break;
            case ResourceType.Steel:
                amount *= 1u + stats.SteelModifier;
                break;
            default:
                break;
        }
        Bag.Add(type, amount);
    }

    public void EquipItem(Item item)
    {
        EquipedItems[item.Category] = item;
    }

    public void AddCommand(Command command)
    {
        if (Commands.Count >= MaxCommands) Commands.RemoveFirst();
        Commands.AddLast(command);
    }

    public static PlayerCharacter CreateDefault()
    {
        return new()
        {
            Name = Constants.Default,
            MaxHealth = 30,
            CurrentHealth = 30,
            BaseStats = new()
            {
                Strength = 15,
                Speed = 2
            },
            MaxCommands = 1,
            Home = Constants.BasePosition,
            Pos = Constants.BasePosition,
        };
    }
    public static PlayerCharacter CreateNewPeasant(Vector3 pos)
    {
        return new()
        {
            Name = Constants.Peasant.MakeNameRandomized(),
            MaxHealth = 20,
            CurrentHealth = 20,
            BaseStats = new()
            {
                Strength = Random.Shared.NextUShort(5, 16),
                Speed = 1,
            },
            MaxCommands = 1,
            Home = pos,
            Pos = pos
        };
    }
    public static PlayerCharacter CreateNewWorker(Vector3 pos)
    {
        return new()
        {
            Name = Constants.Worker.MakeNameRandomized(),
            MaxHealth = 50,
            CurrentHealth = 50,
            BaseStats = new()
            {
                Strength = Random.Shared.NextUShort(6, 15),
                Defense = Random.Shared.NextUShort(5, 11),
                Speed = 2,
            },
            MaxCommands = 3,
            Home = pos,
            Pos = pos
        };
    }
    public static PlayerCharacter CreateNewElite(Vector3 pos)
    {
        return new()
        {
            Name = Constants.Elite.MakeNameRandomized(),
            MaxHealth = 100,
            CurrentHealth = 100,
            BaseStats = new()
            {
                Strength = Random.Shared.NextUShort(20, 31),
                Defense = Random.Shared.NextUShort(10, 15),
                Speed = 3,
            },
            MaxCommands = 5,
            Home = pos,
            Pos = pos
        };
    }
}
