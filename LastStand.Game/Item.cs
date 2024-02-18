using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LastStand.Game;

[JsonConverter(typeof(ItemConverter))]
public class Item : Stats
{
    private static List<Item> items { get; set; } = [
        // Tools
        new Item() { Category = ItemCategory.Main, Name = Constants.Sickle, GrainModifier = 1, Cost = new() { Grain = 100 } }, // + 100% grain
        new Item() { Category = ItemCategory.Main, Name = Constants.Axe, WoodModifier = 1, Cost = new() { Wood = 100, Stone = 50, Steel = 20 } }, // + 100% wood
        new Item() { Category = ItemCategory.Main, Name = Constants.Pickaxe, StoneModifier = 1, SteelModifier = 1, Cost = new() { Wood = 50, Steel = 50 } }, // + 100% stone and steel
        new Item() { Category = ItemCategory.Main, Name = Constants.Scythe, GrainModifier = 2, Cost = new() { Grain = 200, Stone = 50, Steel = 50 } }, // + 200% grain

        // Melee Weapons
        new Item() { Category = ItemCategory.Main, Name = Constants.Dagger, Damage = 4, AttackSpeed = 2, Pierce = 3, Cost = new() { Wood = 30 } }, // fast low damage but pierces
        new Item() { Category = ItemCategory.Main, Name = Constants.Sword, Damage = 8, Pierce = 2, Cost = new() { Wood = 30 } }, // fast high damage no pierce
        new Item() { Category = ItemCategory.Main, Name = Constants.Hammer, Damage = 12, Cost = new() { Stone = 30 } }, // slow high damage no pierce
        new Item() { Category = ItemCategory.Main, Name = Constants.Spear, Damage = 5, Pierce = 5, Cost = new() { Wood = 20 ,Steel = 5 } }, // slow low damage but pierces, lance img
        new Item() { Category = ItemCategory.Main, Name = Constants.SwordOfBleeding, Damage = 15, Pierce = 10, DamageTakenModifier = 2 }, // high damage but also receive 2x damage, rapier img

        // Ranged Weapons
        new Item() { Category = ItemCategory.Main, Name = Constants.Sling, Damage = 3, AttackRange = 2, Cost = new() { Grain = 30 } }, // low damage low range
        new Item() { Category = ItemCategory.Main, Name = Constants.Shortbow, Damage = 5, Pierce = 2, AttackRange = 3, Cost = new() { Wood = 50 } },
        new Item() { Category = ItemCategory.Main, Name = Constants.Longbow, Damage = 10, AttackRange = 6, Cost = new() { Wood = 50 } },
        new Item() { Category = ItemCategory.Main, Name = Constants.Crossbow, Damage = 10, Pierce = 5, AttackRange = 4, Cost = new() { Steel = 40 } },

        // Additional
        new Item() { Category = ItemCategory.Additional, Name = Constants.WoodenShield, Defense = 10, Block = 5, Cost = new() { Wood = 50 } }, // provides defense and block
        new Item() { Category = ItemCategory.Additional, Name = Constants.IronShield, Defense = 20, Block = 10, Cost = new() { Steel = 30 } }, // provides defense and block
        // Additional (Not craftable)
        new Item() { Category = ItemCategory.Additional, Name = Constants.CloakOfProtection, Defense = 40, Block = 5 }, // provides block, cape img
        new Item() { Category = ItemCategory.Additional, Name = Constants.BottomlessBag, CarryModifier = 2 }, // increase carry capacity, purse img
        new Item() { Category = ItemCategory.Additional, Name = Constants.RingOfSpeed, Speed = 3, AttackSpeed = 2 }, // provides speed, ring img
        new Item() { Category = ItemCategory.Additional, Name = Constants.BeltOfStrength, Strength = 50 }, // provides strength, belt img
        new Item() { Category = ItemCategory.Additional, Name = Constants.Horse, Speed = 2, CarryModifier = 1 }, // increase carry capacity and speed, horse img

        // Basic Armor
        new Item() { Category = ItemCategory.Head, Name = Constants.Cap, Strength = 3, Cost = new() { Grain = 30 } },
        new Item() { Category = ItemCategory.Body, Name = Constants.Robe, Defense = 5, Cost = new() { Grain = 50 } },
        new Item() { Category = ItemCategory.Hands, Name = Constants.Gloves, Block = 2, Cost = new() { Grain = 20 } },
        new Item() { Category = ItemCategory.Legs, Name = Constants.Pants, Defense = 5, Cost = new() { Grain = 50 } },
        new Item() { Category = ItemCategory.Feet, Name = Constants.Shoes, Defense = 5, Cost = new() { Grain = 20 } },

        // Leather Armor
        new Item() { Category = ItemCategory.Head, Name = Constants.LeatherHelmet, Strength = 5, Cost = new() { Wood = 30 } },
        new Item() { Category = ItemCategory.Body, Name = Constants.LeatherArmor, Defense = 10, Cost = new() { Wood = 50 } },
        new Item() { Category = ItemCategory.Hands, Name = Constants.LeatherGloves, Defense = 10, Cost = new() { Wood = 20 } },
        new Item() { Category = ItemCategory.Legs, Name = Constants.LeatherPants, Block = 3, Cost = new() { Wood = 50 } },
        new Item() { Category = ItemCategory.Feet, Name = Constants.LeatherBoots, Defense = 10, Speed = 1, Cost = new() { Wood = 20 } },

        // Plate Armor
        new Item() { Category = ItemCategory.Head, Name = Constants.PlateHelmet, Strength = 10, Block = 3, Cost = new() { Steel = 30 } },
        new Item() { Category = ItemCategory.Body, Name = Constants.PlateArmor, Defense = 30, Cost = new() { Steel = 50 } },
        new Item() { Category = ItemCategory.Hands, Name = Constants.PlateGloves, Block = 5, Cost = new() { Steel = 20 } },
        new Item() { Category = ItemCategory.Legs, Name = Constants.PlatePants, Defense = 20, Block = 5, Cost = new() { Steel = 50 } },
        new Item() { Category = ItemCategory.Feet, Name = Constants.PlateBoots, Defense = 10, Block = 3, Speed = 1, Cost = new() { Steel = 20 } }
    ];

    public static FrozenDictionary<string, Item> Items = items.ToFrozenDictionary(x => x.Name, x => x);

    public static FrozenDictionary<int, string[]> LootingTable { get; set; } = new Dictionary<int, string[]>() {
        { 2, [Constants.Robe, Constants.Pants, Constants.Dagger, Constants.Hammer, Constants.Sling]},
        { 3, [Constants.LeatherArmor, Constants.LeatherPants, Constants.Sword, Constants.Spear, Constants.Shortbow, Constants.WoodenShield, Constants.BottomlessBag]},
        { 4, [Constants.IronShield, Constants.Longbow, Constants.Crossbow, Constants.SwordOfBleeding, Constants.CloakOfProtection, Constants.Horse]},
        { 5, [Constants.PlateArmor, Constants.PlatePants, Constants.BeltOfStrength, Constants.RingOfSpeed]},
    }.ToFrozenDictionary();
    public string Name { get; private set; }
    public ItemCategory Category { get; private set; }

    public Resources? Cost { get; private set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemCategory
{
    Main,
    Additional,
    Head,
    Body,
    Hands,
    Legs,
    Feet
}

public class ItemConverter : JsonConverter<Item>
{
    public override Item? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var itemName = reader.GetString();
        return Item.Items[itemName];
    }

    public override void Write(Utf8JsonWriter writer, Item value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}