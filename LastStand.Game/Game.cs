using System.Text.Json.Serialization;

namespace LastStand.Game;
public class Game
{
    public Game()
    {
        Reset();
    }

    public string Name { get; set; }
    public uint RoundHighScore { get; set; }
    public uint Round { get; set; }
    public uint AttackersKilled { get; set; }
    public uint Timer { get; set; } = 1;
    public Map GameMap { get; set; } = new Map();
    public string Status { get; set; }
    public bool GameActive => Status is Constants.Start;
    public uint BaseHealth { get; set; }

    public Resources Resources { get; set; } = new();
    public Dictionary<string, uint> Inventory { get; set; } = [];
    public Dictionary<string, uint> Buildings { get; set; } = [];

    public List<PlayerCharacter> Players { get; set; } = [];
    [JsonIgnore]
    public IEnumerable<PlayerCharacter> ActivePlayers => Players.Where(x => x.Active);
    [JsonIgnore]
    public IEnumerable<PlayerCharacter> ActiveAndAlivePlayers => Players.Where(x => x.Active && x.Dead is false);
    public List<Attacker> Attackers { get; set; } = [];
    [JsonIgnore]
    public IEnumerable<Attacker> AliveAttackers => Attackers.Where(x => x.Dead is false);
    public List<StaticCharacter> StaticCharacters { get; set; } = [];

    public bool CraftingEnabled { get; set; } = false;
    public bool LootingEnabled { get; set; } = false;

    public HashSet<string> Events { get; set; } = [];

    public void RunRound()
    {
        Round++;
        CleanUpDeadAttackers();
        AttackersAct();
        if (BaseHealth <= 0)
        {
            GameOver();
            return;
        }
        PlayersAct();
        StaticCharactersAct();
        MapAct();
        AttackerWave();
    }

    private void CleanUpDeadAttackers()
    {
        var deadAttackers = Attackers.FindAll(x => x.Dead);
        AttackersKilled += (uint)deadAttackers.Count;
        if (LootingEnabled)
        {
            foreach (var attacker in deadAttackers)
            {
                if (attacker.Tier is 1) continue;
                var table = Item.LootingTable[attacker.Tier];
                var item = table[Random.Shared.Next(0, table.Length)];
                AddItem(item);
            }
            Attackers.RemoveAll(x => x.Dead);
        }
        else Attackers.RemoveAll(x => x.Dead);
    }
    private void AttackersAct()
    {
        foreach (var attacker in Attackers)
        {
            if (attacker.Commands.TryPeek(out var command))
            {
                command.Act(this, attacker);
            }
        }
    }
    private void PlayersAct()
    {
        foreach (var player in ActivePlayers)
        {
            if (player.Commands.TryPeek(out var command))
            {
                command.Act(this, player);
            }
            else if (player.IsHome is false)
            {
                ReturnHome(player);
                if (player.IsHome)
                {
                    player.Sleep();
                    Events.Add(Constants.PlayerReturned);
                }
            }
            else
            {
                player.Sleep();
                Events.Add(Constants.PlayerReturned);
            }
        }
    }
    private void StaticCharactersAct()
    {
        foreach (var character in StaticCharacters)
        {
            if (character.Commands.TryPeek(out var command))
            {
                command.Act(this, character);
            }
        }
    }
    private void MapAct()
    {
        if (Round % 100 == 0)
        {
            PopulateTiles();
        }
    }
    public void PopulateTiles()
    {
        foreach (var tile in GameMap.MapTiles.Values)
        {
            tile.Populate();
        }
    }
    public void HandleCommand(string command)
    {
        var action = command.Split(' ').FirstOrDefault();
        switch (action)
        {
            case Constants.Player:
                PlayerControl(command);
                break;
            case Constants.Game:
                GameControl(command);
                break;
            case Constants.Craft:
                CraftControl(command);
                break;
            case Constants.Build:
                BuildControl(command);
                break;
            case Constants.Timer:
                TimerControl(command);
                break;
            default:
                throw new InvalidCommandException("Invalid command");
        }
    }

    public void SendOutPlayer(string name, CombatBehaviour behaviour, List<string> commands, List<string> items)
    {
        // Check player exists
        var player = Players.Find(x => x.Name == name);
        if (player is null) throw new InvalidCommandException($"No player found with name: {name}");
        else if (player.Active) throw new InvalidCommandException($"You cannot command player ({name}) because they have been sent out");
        // Set behaviour
        player.CombatBehaviour = behaviour;
        // Unequip items
        foreach (var item in player.EquipedItems.Values)
        {
            AddItem(item.Name);
        }
        player.EquipedItems.Clear();
        // Equip Items
        foreach (var i in items)
        {
            if (Inventory.TryGetValue(i, out uint itemCount) && itemCount > 0)
            {
                if (Item.Items.TryGetValue(i, out var item))
                {
                    if (player.EquipedItems.TryGetValue(item.Category, out var equipedItem))
                    {
                        AddItem(equipedItem.Name);
                    }
                    player.EquipItem(item);
                    Inventory[i] = itemCount - 1;
                }
                else throw new InvalidCommandException($"Item does not exist: {i}");
            }
            else throw new InvalidCommandException($"You do not have the item: {i}");
        }
        // Set commands
        foreach (var command in commands)
        {
            var parts = command.Split(' ');
            var action = parts[0];
            string? target = parts.Length is 2 ? parts[1] : null;
            switch (action)
            {
                case Constants.Collect:
                    if (target is not null)
                    {
                        if (target is Constants.Grain or Constants.Wood or Constants.Stone or Constants.Steel)
                        {
                            player.AddCommand(new CollectCommand(target));
                        }
                        else throw new InvalidCommandException($"Invalid resource specified: {target}");
                        break;
                    }
                    else throw new InvalidCommandException($"No resource specified provided for collect");
                case Constants.Attack:
                    player.AddCommand(new AttackCommand(target));
                    break;
                case Constants.Kill:
                    player.AddCommand(new KillCommand(target));
                    break;
                case Constants.Rest:
                    player.AddCommand(new RestCommand());
                    break;
                default:
                    throw new InvalidCommandException($"Invalid action: {command}");
            }
        }
        // Send out
        player.SendOut();
    }

    private void PlayerControl(string command)
    {
        var parts = command.Split(' ');
        if (parts.Length is not 3 and not 4) throw new InvalidCommandException($"Invalid player command: {command}");
        var name = parts[1];
        var player = Players.Find(x => x.Name == name);
        if (player is null) throw new InvalidCommandException($"No player found with name: {name}");
        else if (player.Active) throw new InvalidCommandException($"You cannot command player ({name}) because they have been sent out");
        var action = parts[2];
        string? target = parts.Length is 4 ? parts[3] : null;
        switch (action)
        {
            case Constants.Collect:
                if (target is not null)
                {
                    if (target is Constants.Grain or Constants.Wood or Constants.Stone or Constants.Steel)
                    {
                        player.AddCommand(new CollectCommand(target));
                    }
                    else throw new InvalidCommandException($"Invalid resource specified: {target}");
                    break;
                }
                else throw new InvalidCommandException($"No resource specified provided for collect");
            case Constants.Attack:
                player.AddCommand(new AttackCommand(target));
                break;
            case Constants.Kill:
                player.AddCommand(new KillCommand(target));
                break;
            case Constants.Rest:
                player.AddCommand(new RestCommand());
                break;
            case Constants.Equip:
                if (Inventory.TryGetValue(target, out uint itemCount) && itemCount > 0)
                {
                    if (Item.Items.TryGetValue(target, out var item))
                    {
                        if (player.EquipedItems.TryGetValue(item.Category, out var equipedItem))
                        {
                            AddItem(equipedItem.Name);
                        }
                        player.EquipItem(item);
                        Inventory[target] = itemCount - 1;
                        break;
                    }
                    else throw new InvalidCommandException($"Item does not exist: {target}");
                }
                else throw new InvalidCommandException($"You do not have the item: {target}");
            case Constants.Unequip:
                foreach (var item in player.EquipedItems.Values)
                {
                    AddItem(item.Name);
                }
                player.EquipedItems.Clear();
                break;
            case Constants.Behaviour:
                if (Enum.TryParse<CombatBehaviour>(target, true, out var behaviour))
                {
                    player.CombatBehaviour = behaviour;
                    break;
                }
                else throw new InvalidCommandException($"Invalid behaviour: {target}");
            case Constants.Rename:
                if (target is not null)
                {
                    RenamePlayer(player, target);
                    break;
                }
                else throw new InvalidCommandException($"No desired name provided for rename");
            case Constants.Send:
                player.SendOut();
                break;
            default:
                throw new InvalidCommandException($"Invalid action: {action}");
        }
    }

    // Rename a player
    private void RenamePlayer(PlayerCharacter player, string desiredName)
    {
        if (desiredName is null) throw new InvalidCommandException($"Name cannot be empty");
        else if (Players.Any(x => x.Name == desiredName)) throw new InvalidCommandException($"Name {desiredName} is already in use");
        player.Name = desiredName;
    }


    // Returns the player (towards) home but does not sleep
    public void ReturnHome(PlayerCharacter player)
    {
        var path = GameMap.PathFind(player.Pos, player.Home, player.Speed);
        player.MoveTowards(path);
        if (player.IsHome)
        {
            Resources.Add(player.Bag);
            player.BackHome();
        }
    }

    private void AttackerWave()
    {
        if (Round % 75 == 0)
        {
            int wavePower = (int)Math.Floor(Round / 40d);
            // Random sizes, can summon a lot of attackers
            while (wavePower > 0)
            {
                int power = Random.Shared.Next(1, wavePower + 1);
                SummonAttacker(power);
                wavePower -= power;
            }
            Events.Add(Constants.AttackerAppeared);
        }
    }

    private void SummonAttacker(int power)
    {
        while (power > 0)
        {
            ushort tier;
            switch (power)
            {
                case >= Constants.Tier5Cost:
                    tier = 5;
                    power -= Constants.Tier5Cost;
                    break;
                case >= Constants.Tier4Cost:
                    tier = 4;
                    power -= Constants.Tier4Cost;
                    break;
                case >= Constants.Tier3Cost:
                    tier = 3;
                    power -= Constants.Tier3Cost;
                    break;
                case >= Constants.Tier2Cost:
                    tier = 2;
                    power -= Constants.Tier2Cost;
                    break;
                default:
                    tier = 1;
                    power -= Constants.Tier1Cost;
                    break;
            }
            string[] attackers = Attacker.AttackersByTier[tier];
            var attacker = Attacker.Create(attackers[Random.Shared.Next(0, attackers.Length)]);
            attacker.Pos = GameMap.GetRandomBorderPos();
            Attackers.Add(attacker);
        }
    }

    /// <summary>
    /// 'build' controls
    /// </summary>
    /// <param name="building">The building to build</param>
    private void BuildControl(string command)
    {
        string name = command.Split(' ').Last();
        if (Building.Buildings.TryGetValue(name, out var building))
        {
            if (building.Single && GameMap.BuildingExists(building.Name)) throw new InvalidCommandException("You have already built this building");
            Resources cost;
            if (building.Scaling)
            {
                var amount = Buildings.TryGetValue(building.Name, out uint value) ? value : 0;
                cost = building.Cost * (uint)Math.Pow(2, amount); // Cost doubles per building
            }
            else cost = building.Cost;
            if (Resources.HasSufficientResources(cost) is false) throw new InvalidCommandException($"You have insufficient resources to build {name}");
            Resources -= cost;
            var availableTile = GameMap.GetBuildGround();
            availableTile.BuildBuilding(building);
            building.Action(this, availableTile.Pos);
            AddBuilding(building.Name);
        }
        else throw new InvalidCommandException($"Building name invalid: {name}");
    }
    private void AddBuilding(string building)
    {
        var current = Buildings.TryGetValue(building, out uint value) ? value : 0;
        Buildings[building] = current + 1;
    }

    /// <summary>
    /// 'build' controls
    /// </summary>
    /// <param name="building">The building to build</param>
    private void GameOver()
    {
        Status = Constants.GameOver;
        Events.Add(Constants.GameEnded);
    }

    /// <summary>
    /// 'game' controls
    private void CraftControl(string command)
    {
        if (CraftingEnabled is false) throw new InvalidCommandException($"You require a {Constants.Barracks} before you can craft");
        string name = command.Split(' ').Last();
        if (Item.Items.TryGetValue(name, out var item))
        {
            if (item.Cost is null) throw new InvalidCommandException("Item is not craftable");
            else if (Resources.HasSufficientResources(item.Cost) is false) throw new InvalidCommandException($"Insufficient resources to craft {name}");
            Resources -= item.Cost;
            AddItem(item.Name);
        }
        else throw new InvalidCommandException($"Item name invalid: {name}");
    }
    private void AddItem(string item)
    {
        var current = Inventory.TryGetValue(item, out uint value) ? value : 0;
        Inventory[item] = current + 1;
    }

    public void DamageBase(Character attacker)
    {
        // Attacker does only 1 attack at the base
        var attackingStats = attacker.GetStats();
        uint damage = 0;
        uint counter = 0;
        for (uint j = attackingStats.Strength; j >= attackingStats.Damage; j -= attackingStats.Damage)
        {
            damage += attackingStats.Damage - counter;
            counter++;
            if (attackingStats.Damage == counter) break;
        }
        BaseHealth -= Math.Min(damage, BaseHealth);
        Events.Add(Constants.BaseAttacked);
        attacker.TotalDamageDone += damage;
    }

    // End game
    /// - reset = reset to intitial state
    /// - start = start processing
    /// - stop = stop processing
    /// </summary>
    /// <param name="status">The status to set</param>
    private void GameControl(string command)
    {
        var status = command.Split(' ').Last();
        if (status is Constants.Reset) Reset();
        else if (Status is Constants.GameOver) return;
        else if (status is Constants.Start) Status = Constants.Start;
        else if (status is Constants.Stop) Status = Constants.Stop;
        else throw new InvalidCommandException($"Invalid game command: {status}");
    }
    // 'timer' controls simply provide a number in seconds for the length of each round
    // timer 5, timer 1
    private void TimerControl(string command)
    {
        var part = command.Split(' ');
        if (uint.TryParse(part.Last(), out var timer) && timer > 0)
        {
            Timer = timer;
            Events.Add(Constants.TimerChanged);
        }
        else throw new InvalidCommandException("Invalid timer value! It must be a positive integer and greater than 0");
    }

    // Reset to default state, only thing that transfers is highscore and timer
    public void Reset()
    {
        RoundHighScore = Math.Max(RoundHighScore, Round);
        Round = 0;
        AttackersKilled = 0;
        Status = Constants.Initial;
        BaseHealth = Constants.BaseHealth;
        GameMap.CreateMap();
        MapAct();
        Resources.Reset();
        Players.Clear();
        Attackers.Clear();
        StaticCharacters.Clear();
        Inventory.Clear();
        Buildings.Clear();
        CraftingEnabled = false;
        LootingEnabled = false;

        var defaultPlayer = PlayerCharacter.CreateDefault();
        defaultPlayer.CalculateDerivedStats();
        Players.Add(defaultPlayer);
        Events.Add(Constants.PlayerReturned);
    }
}
