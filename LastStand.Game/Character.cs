using System.Numerics;
using System.Text.Json.Serialization;
using static LastStand.Game.Extensions;

namespace LastStand.Game;
public class Character
{
    public required string Name { get; set; }
    public bool Boy => (Name.GetHashCode() % 2) is not 0;

    [JsonConverter(typeof(VectorJsonConverter))]
    public Vector3 Pos { get; set; }

    public required uint MaxHealth { get; set; }
    public required uint CurrentHealth { get; set; } = 1;
    public Stats BaseStats { get; set; } = new Stats();
    public virtual Stats GetStats() => BaseStats;
    [JsonIgnore]
    public uint AttackRange => GetStats().AttackRange;
    [JsonIgnore]
    public uint Speed => GetStats().Speed;
    public bool Dead => CurrentHealth <= 0;

    public CombatBehaviour CombatBehaviour { get; set; } = CombatBehaviour.ignore;
    public LinkedList<Command> Commands { get; set; } = new();

    public uint TotalDamageDone { get; set; }

    public void MoveTowards(IList<Vector3> path)
    {
        if (path.Count == 0) return;
        var steps = Speed;
        var target = path[^1];
        for (int i = 0; i < path.Count; i++)
        {
            Pos = path[i];
            if (Pos == target) break;
            steps--;
            if (steps <= 0) break;
        }
    }

    public void TakeDamage(Character attacker)
    {
        var defendingStats = GetStats();
        var attackingStats = attacker.GetStats();
        for (int i = 0; i < attackingStats.AttackSpeed; i++)
        {
            uint damage = 0;
            uint counter = 0;
            for (uint j = attackingStats.Strength; j >= attackingStats.Damage; j -= attackingStats.Damage)
            {
                damage += attackingStats.Damage - counter;
                counter++;
                if (attackingStats.Damage == counter) break;
            }
            if (damage is 0) return;

            // Reduce damage by defense
            var reducedDamage = damage - (damage * (defendingStats.Defense / 100));

            // Damage taken modifier
            reducedDamage *= 1 + defendingStats.DamageTakenModifier;

            uint pierceDamage = Math.Max(attackingStats.Pierce - attackingStats.Block, 0);
            var finalCalcDamage = reducedDamage + pierceDamage;

            if (reducedDamage is 0) return;

            // Apply damage to health

            var finalDamage = Math.Min(finalCalcDamage, CurrentHealth);
            CurrentHealth -= finalDamage;
            attacker.TotalDamageDone += finalDamage;
        }
        HandleCombatBehaviour(attacker);
    }

    private void HandleCombatBehaviour(Character character)
    {
        if (CombatBehaviour is CombatBehaviour.ignore) return;
        if (CombatBehaviour is CombatBehaviour.flee) Commands.Clear();
        else if (CombatBehaviour is CombatBehaviour.fight)
        {
            if (Commands.Any(x => x is KillCommand && x.Target == character.Name)) return;
            else if (Pos.Distance(character.Pos) > 1) return;
            else Commands.AddFirst(new KillCommand(character.Name));
        }
    }

    public void RemoveCommand()
    {
        Commands.RemoveFirst();
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CombatBehaviour
{
    ignore = 0, // ignore being attacked
    flee = 1, // drop all commands and returns home
    fight = 2 // stand and fight any melee enemies that attack
}