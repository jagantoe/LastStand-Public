namespace LastStand.Game;
public class Stats
{
    public uint Strength { get; set; } // Scales your Damage and CarryLimit
    public uint Defense { get; set; } // Percentage of damage reduction
    public uint Speed { get; set; }

    public uint Damage { get; set; } = 1;
    public uint AttackSpeed { get; set; } = 1;
    public uint AttackRange { get; set; } = 1;
    public uint Pierce { get; set; } // Flat damage added after calculation
    public uint Block { get; set; } // Flat reduces pierce damage
    public uint DamageTakenModifier { get; set; }

    // Modifiers are added to base 1
    public uint GrainModifier { get; set; }
    public uint WoodModifier { get; set; }
    public uint StoneModifier { get; set; }
    public uint SteelModifier { get; set; }

    public uint CarryModifier { get; set; }

    public static Stats operator +(Stats stats1, Stats stats2)
    {
        return new()
        {
            Strength = stats1.Strength + stats2.Strength,
            Defense = stats1.Defense + stats2.Defense,
            Speed = stats1.Speed + stats2.Speed,

            Damage = Math.Max(stats1.Damage, stats2.Damage),
            AttackSpeed = Math.Max(stats1.AttackSpeed, stats2.AttackSpeed),
            AttackRange = Math.Max(stats1.AttackRange, stats2.AttackRange),
            Pierce = Math.Max(stats1.Pierce, stats2.Pierce),

            Block = stats1.Block + stats2.Block,
            DamageTakenModifier = stats1.DamageTakenModifier = stats2.DamageTakenModifier,

            GrainModifier = stats1.GrainModifier + stats2.GrainModifier,
            WoodModifier = stats1.WoodModifier + stats2.WoodModifier,
            StoneModifier = stats1.StoneModifier + stats2.StoneModifier,
            SteelModifier = stats1.SteelModifier + stats2.SteelModifier,

            CarryModifier = stats1.CarryModifier + stats2.CarryModifier
        };
    }

    public static Stats operator +(Stats stats1, IEnumerable<Stats> stats2)
    {
        var stats = stats1;
        foreach (var s in stats2)
        {
            stats += s;
        }
        return stats;
    }
}
