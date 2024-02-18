using System.Text.Json;
using System.Text.Json.Serialization;

namespace LastStand.Game;

[JsonConverter(typeof(CommandConverter))]
public abstract class Command(string Name, string? Target = null, int Timer = 0)
{
    public string Name { get; set; } = Name;
    public string? Target { get; set; } = Target;
    public int Timer { get; set; } = Timer;

    public abstract void Act(Game game, Character actor);
}

public class CollectCommand(string target) : Command(Constants.Collect, target)
{
    public override void Act(Game game, Character actor)
    {
        var target = Enum.Parse<ResourceType>(Target!, true);
        if (actor is PlayerCharacter player && player.Bag.Full(target) is false && player.Speed is not 0)
        {
            var speed = player.Speed;
            while (speed > 0)
            {
                if (player.Bag.Full(target) is false && game.GameMap.TryFindNearest(player.Pos, x => x.Resource?.ResourceType == target, out var pos))
                {
                    uint distance = (uint)player.Pos.Distance(pos);
                    if (distance > speed) // Target out of range so only move
                    {
                        var path = game.GameMap.PathFind(player.Pos, pos, speed);
                        player.MoveTowards(path);
                        return;
                    }
                    else // Target in range so move and collect it
                    {
                        var path = game.GameMap.PathFind(player.Pos, pos, speed);
                        player.MoveTowards(path);
                        game.GameMap.MapTiles[pos].Resource!.Collect(player);
                        if (target is ResourceType.Grain) speed -= distance;
                        else return;
                    }
                }
                else
                {
                    player.RemoveCommand();
                    return;
                }
            }
        }
        else actor.RemoveCommand(); // Not a player or bag full
    }
}

// Attack a target
public class AttackCommand(string? target = null) : Command(Constants.Attack, target)
{
    public override void Act(Game game, Character actor)
    {
        Character? target = null;
        if (Target is null) target = game.AliveAttackers.MinBy(p => p.Pos.Distance(actor.Pos))!;
        else target = game.AliveAttackers.FirstOrDefault(x => x.Name == Target);
        if (target is null)
        {
            actor.RemoveCommand();
            return;
        }
        if (target.Pos.Distance(actor.Pos) <= actor.AttackRange)
        {
            target.TakeDamage(actor);
            actor.RemoveCommand();
        }
        else
        {
            var path = game.GameMap.PathFind(actor.Pos, target.Pos, actor.Speed);
            actor.MoveTowards(path);
        }
    }
}

// Fight a target until it is dead
public class KillCommand(string? target = null) : Command(Constants.Kill, target)
{
    public override void Act(Game game, Character actor)
    {
        Character? target = null;
        if (Target is null) target = game.AliveAttackers.MinBy(p => p.Pos.Distance(actor.Pos))!;
        else target = game.AliveAttackers.FirstOrDefault(x => x.Name == Target);
        if (target is null)
        {
            actor.RemoveCommand();
            return;
        }
        if (target.Pos.Distance(actor.Pos) <= actor.AttackRange)
        {
            target.TakeDamage(actor);
            if (target.Dead) actor.RemoveCommand();
        }
        else
        {
            var path = game.GameMap.PathFind(actor.Pos, target.Pos, actor.Speed);
            actor.MoveTowards(path);
        }
    }
}

// Player heals at home, if not home they return home
public class RestCommand() : Command(Constants.Rest)
{
    public override void Act(Game game, Character actor)
    {
        if (actor is PlayerCharacter player)
        {
            if (player.IsHome)
            {
                player.Heal();
                player.RemoveCommand();
            }
            else game.ReturnHome(player);
        }
        else actor.RemoveCommand();
    }
}

// On death a player goes into stasis for a while before they become available again
public class StasisCommand(int timer) : Command(Constants.Stasis, null, timer)
{
    public override void Act(Game game, Character actor)
    {
        if (Timer > 0) Timer--;
        else
        {
            actor.RemoveCommand();
            if (actor is PlayerCharacter player) player.Revive();
        }
    }
}

// Attack the nearest character to attack, if there are none they walk in circles around the base
public class OnlyAttackCommand() : Command(Constants.OnlyAttack)
{
    public override void Act(Game game, Character actor)
    {
        IEnumerable<Character> possibleTargets;
        if (actor is Attacker) possibleTargets = game.ActiveAndAlivePlayers;
        else if (actor is PlayerCharacter || actor is StaticCharacter) possibleTargets = game.AliveAttackers;
        else possibleTargets = [];
        if (possibleTargets.Count() is 0) // No active targets so move in circle
        {
            ushort distance = (ushort)actor.Pos.Distance(Constants.BasePosition);
            if (distance is 0 || actor.Speed is 0) return;
            var ring = Map.OrderedHexRings[distance];
            var index = ring.IndexOf(actor.Pos) + 1;
            if (index >= ring.Count) index %= ring.Count;
            var path = ring[index..];
            actor.MoveTowards(path);
            return;
        }
        var nearestTarget = possibleTargets.MinBy(p => p.Pos.Distance(actor.Pos))!;
        if (nearestTarget.Pos.Distance(actor.Pos) <= actor.AttackRange) // Attack nearest target
        {
            nearestTarget.TakeDamage(actor);
            if (nearestTarget is PlayerCharacter player && player.Dead) player.Die();
        }
        else // No target in range so move towards nearest target
        {
            if (actor.Speed is 0) return;
            var path = game.GameMap.PathFind(actor.Pos, nearestTarget.Pos, actor.Speed);
            actor.MoveTowards(path);
        }
    }
}

// Attacker goes for the base and attacks it until it's destroyed, if there is a player within range they attack the player
public class DestroyBaseCommand() : Command(Constants.DestroyBase)
{
    public override void Act(Game game, Character actor)
    {
        var player = game.ActiveAndAlivePlayers.Where(p => p.Pos.Distance(actor.Pos) <= actor.AttackRange).MinBy(x => x.Pos.Distance(actor.Pos));
        if (player is null) // No player in range move to base
        {
            if (actor.Pos.Distance(Constants.BasePosition) <= actor.AttackRange)
            {
                game.DamageBase(actor);
            }
            else
            {
                var path = game.GameMap.PathFind(actor.Pos, Constants.BasePosition, actor.Speed);
                actor.MoveTowards(path);
            }
        }
        else // Attack player in range
        {
            player.TakeDamage(actor);
            if (player.Dead) player.Die();
        }
    }
}

public class CommandConverter : JsonConverter<Command>
{
    public override Command? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var commandName = reader.GetString();
        string[] parts = commandName.Split("-");
        string command = parts[0];
        string target = parts[1];
        int timer = int.Parse(parts[2]);
        return command switch
        {
            Constants.Collect => new CollectCommand(target),
            Constants.Attack => new AttackCommand(target),
            Constants.Kill => new KillCommand(target),
            Constants.Rest => new RestCommand(),
            Constants.Stasis => new StasisCommand(timer),
            Constants.OnlyAttack => new OnlyAttackCommand(),
            Constants.DestroyBase => new DestroyBaseCommand(),
            _ => throw new LastStandException("Invalid command detected during parsing"),
        };
    }

    public override void Write(Utf8JsonWriter writer, Command value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{value.Name}-{value.Target}-{value.Timer}");
    }
}