using System.Numerics;

namespace LastStand.Game;

public static class Constants
{
    // Game Status
    public const string Initial = "initial";
    public const string Start = "start";
    public const string Stop = "stop";
    public const string GameOver = "gameover";
    public const string Reset = "reset";

    // Resources
    public const string Grain = "grain";
    public const string Wood = "wood";
    public const string Stone = "stone";
    public const string Steel = "steel";

    // Player types
    public const string Default = "default";
    public const string Peasant = "peasant";
    public const string Worker = "worker";
    public const string Elite = "elite";

    // Stats
    public const ushort BaseHealth = 100;
    public static readonly Vector3 BasePosition = Vector3.Zero;
    public const ushort StartMapSize = 6;
    public const ushort MediumMapSize = 12;
    public const ushort LargeMapSize = 18;

    public const int Tier5Cost = 20;
    public const int Tier4Cost = 15;
    public const int Tier3Cost = 10;
    public const int Tier2Cost = 5;
    public const int Tier1Cost = 2;

    public const uint BaseLimit = 50;
    public const uint FarmLimit = 100;
    public const uint WindmillLimit = 250;
    public const uint QuarryLimit = 500;

    // Tier 1 tile growth
    public const float GrainGrowLimit1 = 0.9f; // 70%
    public const float WoodGrowLimit1 = 1f; // 10%
    // Tier 2 tile growth
    public const float GrainGrowLimit2 = 0.7f; // 70%
    public const float WoodGrowLimit2 = 0.89f; // 19%
    public const float StoneGrowLimit2 = 0.99f; // 10%
    public const float SteelGrowLimit2 = 1f; // 1%
    // Tier 3 tile growth
    public const float GrainGrowLimit3 = 0.4f; // 40%
    public const float WoodGrowLimit3 = 0.75f; // 35%
    public const float StoneGrowLimit3 = 0.95f; // 20%
    public const float SteelGrowLimit3 = 1f; // 5%

    // Buildings
    public const string Base = "base";
    public const string Tower = "tower";
    public const string Lighthouse = "lighthouse";
    public const string Barracks = "barracks";
    public const string Farm = "farm";
    public const string Windmill = "windmill";
    public const string Quarry = "quarry";
    public const string Sanctuary = "sanctuary";
    public const string Tent = "tent";
    public const string House = "house";
    public const string CityHouse = "city_house";
    public const string Storage = "storage";
    public const string Ballista = "ballista";
    public const string Trebuchet = "trebuchet";

    // Attackers
    public const string Crab = "crab";
    public const string Boar = "boar";
    public const string Wolf = "wolf";
    public const string Spider = "spider";
    public const string Zombie = "zombie";
    public const string Bear = "bear";
    public const string Gorilla = "gorilla";
    public const string Werewolf = "werewolf";
    public const string GiantWorm = "giant_worm";

    public const string Goblin = "goblin";
    public const string Skeleton = "skeleton";
    public const string Orc = "orc";
    public const string SkeletonWarrior = "skeleton_warrior";
    public const string SkeletonArcher = "skeleton_archer";
    public const string Bandit = "bandit";
    public const string Rogue = "rogue";
    public const string Barbarian = "barbarian";
    public const string Troll = "troll";
    public const string Cyclops = "cyclops";
    public const string Dragon = "dragon";


    // Commands
    public const string Game = "game";
    public const string Timer = "timer";
    public const string Player = "player";
    public const string Build = "build";
    public const string Craft = "craft";

    public const string Rename = "rename";
    public const string Send = "send";
    public const string Equip = "equip";
    public const string Unequip = "unequip";
    public const string Behaviour = "behaviour";

    public const string Collect = "collect";
    public const string Attack = "attack";
    public const string Kill = "kill";
    public const string Rest = "rest";

    public const string Stasis = "statis";
    public const string OnlyAttack = "onlyattack";
    public const string DestroyBase = "destroybase";

    // Events
    public const string PlayerReturned = "player_returned";
    public const string AttackerAppeared = "attacker_appeared";
    public const string BaseAttacked = "base_attacked";
    public const string GameEnded = "game_ended";
    public const string TimerChanged = "timer_changed";

    // Items
    public const string Sickle = "sickle";
    public const string Pickaxe = "pickaxe";
    public const string Scythe = "scythe";
    public const string Axe = "axe";
    public const string Dagger = "dagger";
    public const string Sword = "sword";
    public const string Hammer = "hammer";
    public const string Spear = "spear";
    public const string SwordOfBleeding = "sword_of_bleeding";
    public const string Sling = "sling";
    public const string Shortbow = "shortbow";
    public const string Crossbow = "crossbow";
    public const string Longbow = "longbow";
    public const string Horse = "horse";
    public const string CloakOfProtection = "cloak_of_protection";
    public const string BottomlessBag = "bottomless_bag";
    public const string RingOfSpeed = "ring_of_speed";
    public const string BeltOfStrength = "belt_of_strength";
    public const string WoodenShield = "wooden_shield";
    public const string IronShield = "iron_shield";
    public const string Cap = "basic_cap";
    public const string Robe = "basic_robe";
    public const string Gloves = "basic_gloves";
    public const string Pants = "basic_pants";
    public const string Shoes = "basic_shoes";
    public const string LeatherHelmet = "leather_helmet";
    public const string LeatherArmor = "leather_armor";
    public const string LeatherGloves = "leather_gloves";
    public const string LeatherPants = "leather_pants";
    public const string LeatherBoots = "leather_boots";
    public const string PlateHelmet = "plate_helmet";
    public const string PlateArmor = "plate_armor";
    public const string PlateGloves = "plate_gloves";
    public const string PlatePants = "plate_pants";
    public const string PlateBoots = "plate_boots";
}
