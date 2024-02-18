using System.Collections.Frozen;

namespace LastStand.Game;
public class Attacker : Character
{
    public Attacker()
    {

    }

    public Attacker(Command command)
    {
        CombatBehaviour = CombatBehaviour.ignore;
        Commands.AddLast(command);
    }

    public static Attacker Create(string attacker)
    {
        var name = attacker.MakeNameRandomized();
        return attacker switch
        {
            // Player Attackers
            // 1
            Constants.Crab => new PlayerAttacker() { Tier = 1, Name = name, MaxHealth = 5, CurrentHealth = 5, BaseStats = new() { Speed = 1, Strength = 1, Damage = 1, Pierce = 0, Defense = 0, Block = 0 } },
            // 2
            Constants.Boar => new PlayerAttacker() { Tier = 2, Name = name, MaxHealth = 20, CurrentHealth = 20, BaseStats = new() { Speed = 1, Strength = 4, Damage = 2, Pierce = 0, Defense = 0, Block = 0 } },
            Constants.Wolf => new PlayerAttacker() { Tier = 2, Name = name, MaxHealth = 10, CurrentHealth = 10, BaseStats = new() { Speed = 2, Strength = 6, Damage = 2, Pierce = 1, Defense = 0, Block = 0 } },
            // 3
            Constants.Spider => new PlayerAttacker() { Tier = 3, Name = name, MaxHealth = 30, CurrentHealth = 30, BaseStats = new() { Speed = 1, Strength = 6, Damage = 3, Pierce = 3, Defense = 0, Block = 0 } },
            Constants.Zombie => new PlayerAttacker() { Tier = 3, Name = name, MaxHealth = 50, CurrentHealth = 50, BaseStats = new() { Speed = 1, Strength = 8, Damage = 4, Pierce = 0, Defense = 0, Block = 0 } },
            // 4
            Constants.Bear => new PlayerAttacker() { Tier = 4, Name = name, MaxHealth = 60, CurrentHealth = 60, BaseStats = new() { Speed = 1, Strength = 12, Damage = 4, Pierce = 3, Defense = 0, Block = 0 } },
            Constants.Gorilla => new PlayerAttacker() { Tier = 4, Name = name, MaxHealth = 80, CurrentHealth = 80, BaseStats = new() { Speed = 1, Strength = 18, Damage = 6, Pierce = 0, Defense = 0, Block = 0 } },
            // 5
            Constants.Werewolf => new PlayerAttacker() { Tier = 5, Name = name, MaxHealth = 100, CurrentHealth = 100, BaseStats = new() { Speed = 2, Strength = 25, Damage = 5, Pierce = 5, Block = 20, AttackSpeed = 2 } }, // strong foe
            Constants.GiantWorm => new PlayerAttacker() { Tier = 5, Name = name, MaxHealth = 300, CurrentHealth = 300, BaseStats = new() { Speed = 1, Strength = 20, Damage = 5, Pierce = 0, Defense = 20, Block = 0 } }, // very tanky but low damage

            // Base Attackers
            // 1
            Constants.Goblin => new BaseAttacker() { Tier = 1, Name = name, MaxHealth = 5, CurrentHealth = 5, BaseStats = new() { Speed = 1, Strength = 1, Damage = 1, Pierce = 1, Defense = 0, Block = 0 } },
            Constants.Skeleton => new BaseAttacker() { Tier = 1, Name = name, MaxHealth = 10, CurrentHealth = 10, BaseStats = new() { Speed = 1, Strength = 2, Damage = 2, Pierce = 0, Defense = 0, Block = 0 } },
            // 2
            Constants.Orc => new BaseAttacker() { Tier = 2, Name = name, MaxHealth = 20, CurrentHealth = 20, BaseStats = new() { Speed = 1, Strength = 4, Damage = 4, Pierce = 0, Defense = 0, Block = 0 } },
            Constants.SkeletonWarrior => new BaseAttacker() { Tier = 2, Name = name, MaxHealth = 15, CurrentHealth = 15, BaseStats = new() { Speed = 1, Strength = 8, Damage = 4, Pierce = 0, Defense = 0, Block = 0 } },
            Constants.SkeletonArcher => new BaseAttacker() { Tier = 2, Name = name, MaxHealth = 10, CurrentHealth = 10, BaseStats = new() { Speed = 1, Strength = 9, Damage = 3, Pierce = 2, Defense = 0, Block = 0, AttackRange = 3 } },
            // 3
            Constants.Bandit => new BaseAttacker() { Tier = 3, Name = name, MaxHealth = 30, CurrentHealth = 30, BaseStats = new() { Speed = 1, Strength = 10, Damage = 5, Pierce = 1, Defense = 10, Block = 3 } },
            Constants.Rogue => new BaseAttacker() { Tier = 3, Name = name, MaxHealth = 20, CurrentHealth = 20, BaseStats = new() { Speed = 2, Strength = 12, Damage = 4, Pierce = 3, Defense = 0, Block = 0, AttackSpeed = 3 } },
            Constants.Barbarian => new BaseAttacker() { Tier = 3, Name = name, MaxHealth = 50, CurrentHealth = 50, BaseStats = new() { Speed = 1, Strength = 25, Damage = 5, Defense = 20, Block = 0 } },
            // 4
            Constants.Troll => new BaseAttacker() { Tier = 4, Name = name, MaxHealth = 60, CurrentHealth = 60, BaseStats = new() { Speed = 1, Strength = 18, Damage = 6, Pierce = 0, Defense = 30, Block = 20 } },
            Constants.Cyclops => new BaseAttacker() { Tier = 4, Name = name, MaxHealth = 100, CurrentHealth = 100, BaseStats = new() { Speed = 1, Strength = 20, Damage = 10, Pierce = 0, Defense = 20, Block = 0 } },
            // 5
            Constants.Dragon => new BaseAttacker() { Tier = 5, Name = name, MaxHealth = 200, CurrentHealth = 200, BaseStats = new() { Speed = 3, Strength = 40, Damage = 10, Pierce = 5, Defense = 50, Block = 10 } },
            _ => throw new LastStandException("Invalid attacker created")
        };
    }

    public static FrozenDictionary<int, string[]> AttackersByTier = new Dictionary<int, string[]>()
    {
        { 1, [Constants.Crab, Constants.Goblin, Constants.Skeleton] },
        { 2, [Constants.Boar, Constants.Wolf, Constants.Orc, Constants.SkeletonWarrior, Constants.SkeletonArcher] },
        { 3, [Constants.Spider, Constants.Zombie, Constants.Bandit, Constants.Rogue, Constants.Barbarian] },
        { 4, [Constants.Bear, Constants.Gorilla, Constants.Cyclops, Constants.Troll] },
        { 5, [Constants.Werewolf, Constants.GiantWorm, Constants.Dragon] },
    }.ToFrozenDictionary();

    public int Tier { get; set; }
}

public class BaseAttacker() : Attacker(new DestroyBaseCommand()) { }

public class PlayerAttacker() : Attacker(new OnlyAttackCommand()) { }