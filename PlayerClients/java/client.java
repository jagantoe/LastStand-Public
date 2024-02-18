import java.util.HashMap;
import java.util.ArrayList;

class Game
{
    public String Name;
    public int RoundHighScore;
    public int Round;
    public int Timer;
    public String Status;
    public boolean GameActive;
    public int BaseHealth;
    public Resources Resources ;
    public HashMap<String, Integer> Inventory;
    public ArrayList<Player> Players;
    public ArrayList<Character> Attackers;
    public ArrayList<Character> StaticCharacters;
}

class Character
{
    public String Name;
    public String Pos;
    public int MaxHealth ;
    public int CurrentHealth;
    public Stats BaseStats;
    public boolean Dead;
    public CombatBehaviour CombatBehaviour;
    public ArrayList<Command> Commands;
    public int TotalDamageDone;
    public int DistanceToBase;
}

class Player extends Character
{
    public boolean Active;
    public boolean IsHome;
    public int MaxCommands;
    public Stats DerivedStats;
    public HashMap<String, String> EquipedItems;
    public Resources Bag;
    public int Deaths;
}

class Resources
{
    public int Grain;
    public int Wood;
    public int Stone;
    public int Steel;
    public int Limit;
}

class Stats
{
    public int Strength;
    public int Defense;
    public int Speed;

    public int Damage;
    public int AttackSpeed;
    public int AttackRange;
    public int Pierce;
    public int Block;
    public int DamageTakenModifier;

    public int GrainModifier;
    public int WoodModifier;
    public int StoneModifier;
    public int SteelModifier;

    public int CarryModifier;
}

class Command
{
    public String Name;
    public String Target;
    public int Timer;
}

enum CombatBehaviour
{
    ignore,
    flee,
    fight
}