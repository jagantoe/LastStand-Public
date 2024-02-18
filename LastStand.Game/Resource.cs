using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Random;
namespace LastStand.Game;

[JsonConverter(typeof(ResourceConverter))]
public abstract class Resource(ResourceType type, uint amount)
{
    public ResourceType ResourceType { get; set; } = type;
    public uint Amount { get; set; } = amount;
    public string Name => GetType().Name;
    [JsonIgnore]
    public Tile Tile { get; set; }

    public virtual void Collect(PlayerCharacter player)
    {
        if (player.Bag.Full(ResourceType)) return;
        var amount = Math.Min(Amount, 5);
        Amount -= amount;
        player.PickUpResource(ResourceType, amount);
        if (Amount == 0) Tile.Resource = null;
    }
}
public abstract class HarvestResource(ResourceType type, uint amount) : Resource(type, amount)
{
    public override void Collect(PlayerCharacter player)
    {
        if (player.Bag.Full(ResourceType)) return;
        player.PickUpResource(type, Amount);
        Tile.Resource = null;
    }
}
// Grain
public class SmallGrain() : HarvestResource(ResourceType.Grain, Shared.NextUInt(1, 6)) { }
public class MediumGrain(uint initial = 0) : HarvestResource(ResourceType.Grain, initial + Shared.NextUInt(1, 10)) { }
public class LargeGrain(uint initial = 0) : HarvestResource(ResourceType.Grain, initial + Shared.NextUInt(1, 10)) { }
// Wood
public class Tree() : Resource(ResourceType.Wood, Shared.NextUInt(5, 20)) { }
public class Forest(uint initial = 0) : Resource(ResourceType.Wood, initial + Shared.NextUInt(20, 50)) { }
// Stone
public class StonePile() : Resource(ResourceType.Stone, Shared.NextUInt(20, 50)) { }
// Steel
public class SteelMine() : Resource(ResourceType.Steel, Shared.NextUInt(50, 100)) { }

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResourceType
{
    Grain = 0,
    Wood = 1,
    Stone = 2,
    Steel = 3
}

public class ResourceConverter : JsonConverter<Resource>
{
    public override Resource? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var resourceString = reader.GetString();
        string[] parts = resourceString.Split("-");
        string type = parts[0];
        uint amount = uint.Parse(parts[1]);
        return type switch
        {
            nameof(SmallGrain) => new SmallGrain() { Amount = amount },
            nameof(MediumGrain) => new MediumGrain() { Amount = amount },
            nameof(LargeGrain) => new LargeGrain() { Amount = amount },
            nameof(Tree) => new Tree() { Amount = amount },
            nameof(Forest) => new Forest() { Amount = amount },
            nameof(StonePile) => new StonePile() { Amount = amount },
            nameof(SteelMine) => new SteelMine() { Amount = amount },
            _ => throw new LastStandException("Invalid resource detected during parsing"),
        };
    }

    public override void Write(Utf8JsonWriter writer, Resource value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{value.Name}-{value.Amount}");
    }
}