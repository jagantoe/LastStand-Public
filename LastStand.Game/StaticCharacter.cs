using System.Numerics;

namespace LastStand.Game;
public class StaticCharacter : Character
{
    public static StaticCharacter CreateBallista(Vector3 pos)
    {
        var balista = new StaticCharacter() { Pos = pos, MaxHealth = 1, CurrentHealth = 1, Name = $"{Constants.Ballista} ({pos.Serialize()})", BaseStats = new() { AttackRange = 3, Strength = 12, Damage = 6, Pierce = 5 } };
        balista.Commands.AddLast(new OnlyAttackCommand());
        return balista;
    }

    public static StaticCharacter CreateTrebuchet(Vector3 pos)
    {
        var trebuchet = new StaticCharacter() { Pos = pos, MaxHealth = 1, CurrentHealth = 1, Name = $"{Constants.Trebuchet} ({pos.Serialize()})", BaseStats = new() { AttackRange = 6, Strength = 8, Damage = 4 } };
        trebuchet.Commands.AddLast(new OnlyAttackCommand());
        return trebuchet;
    }
}
