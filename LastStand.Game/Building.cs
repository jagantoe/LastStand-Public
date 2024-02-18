using System.Collections.Frozen;
using System.Numerics;

namespace LastStand.Game;
public class Building
{
    public static FrozenDictionary<string, Building> Buildings = new Building[] {
        // Single
        new Building() { Name = Constants.Base, Single = true }, // fort image
        new Building() { Name = Constants.Tower, Action = BuildTower, Single = true, Cost = new() { Wood = 100 } }, // tower image
        new Building() { Name = Constants.Lighthouse, Action = BuildLighthouse, Single = true, Cost = new() { Grain = 200, Wood = 200, Stone = 100 } }, // lighthouse image

        new Building() { Name = Constants.Barracks, Action = BuildBarrack, Single = true, Cost = new() { Wood = 100 } }, // military tent image
        new Building() { Name = Constants.Sanctuary, Action = BuildSanctuary, Single = true, Cost = new() { Wood = 100, Stone = 200 } }, // sanctuary image
        
        new Building() { Name = Constants.Farm, Action = BuildFarm, Single = true, Cost = new() { Grain = 50 } }, // farm image
        new Building() { Name = Constants.Windmill, Action = BuildWindmill, Single = true, Cost = new() { Grain = 100, Wood = 50 } }, // windmill image
        new Building() { Name = Constants.Quarry, Action = BuildQuarry, Single = true, Cost = new() { Grain = 200, Wood = 100, Stone = 50 } }, // quarry image

        // Multiple
        new Building() { Name = Constants.Tent, Action = BuildTent, Cost = new() { Grain = 50, Wood = 20 } }, // tent image
        new Building() { Name = Constants.House, Action = BuildHouse, Cost = new() { Grain = 100, Wood = 50, Stone = 20 } }, // house image
        new Building() { Name = Constants.CityHouse, Action = BuildCityHouse, Cost = new() { Grain = 200, Wood = 100, Stone = 50, Steel = 20 } }, // city_house image
        
        new Building() { Name = Constants.Ballista, Action = BuildBallista, Cost = new() { Wood = 100, Stone = 50, Steel = 60 } }, // balista image
        new Building() { Name = Constants.Trebuchet, Action = BuildTrebuchet, Cost = new() { Wood = 100, Stone = 100, Steel = 30 } }, // trebuchet image

        new Building() { Name = Constants.Storage, Action = BuildStorage, Cost = new() { Grain = 500, Wood = 500, Stone = 300, Steel = 100 }, Scaling = true }, // chapel image
    }.ToFrozenDictionary(x => x.Name, x => x);

    public string Name { get; set; }
    public bool Single { get; set; }
    public bool Scaling { get; set; } // The cost doubles each time you build this building
    public Resources Cost { get; set; }

    public Action<Game, Vector3> Action { get; set; }

    // Single
    public static void BuildTower(Game game, Vector3 pos)
    {
        game.GameMap.IncreaseMapMedium();
        game.PopulateTiles();
    }
    public static void BuildLighthouse(Game game, Vector3 pos)
    {
        game.GameMap.IncreaseMapLarge();
        game.PopulateTiles();
    }
    public static void BuildBarrack(Game game, Vector3 pos)
    {
        game.CraftingEnabled = true;
    }
    public static void BuildSanctuary(Game game, Vector3 pos)
    {
        game.LootingEnabled = true;
    }
    public static void BuildFarm(Game game, Vector3 pos)
    {
        game.Resources.Limit = Constants.FarmLimit;
    }
    public static void BuildWindmill(Game game, Vector3 pos)
    {
        game.Resources.Limit = Constants.WindmillLimit;
    }
    public static void BuildQuarry(Game game, Vector3 pos)
    {
        game.Resources.Limit = Constants.QuarryLimit;
    }

    // Multiple
    public static void BuildTent(Game game, Vector3 pos)
    {
        var player = PlayerCharacter.CreateNewPeasant(pos);
        player.CalculateDerivedStats();
        game.Players.Add(player);
        game.Events.Add(Constants.PlayerReturned);
    }
    public static void BuildHouse(Game game, Vector3 pos)
    {
        var player = PlayerCharacter.CreateNewWorker(pos);
        player.CalculateDerivedStats();
        game.Players.Add(player);
        game.Events.Add(Constants.PlayerReturned);
    }
    public static void BuildCityHouse(Game game, Vector3 pos)
    {
        var player = PlayerCharacter.CreateNewElite(pos);
        player.CalculateDerivedStats();
        game.Players.Add(player);
        game.Events.Add(Constants.PlayerReturned);
    }
    public static void BuildBallista(Game game, Vector3 pos)
    {
        game.StaticCharacters.Add(StaticCharacter.CreateBallista(pos));
    }
    public static void BuildTrebuchet(Game game, Vector3 pos)
    {
        game.StaticCharacters.Add(StaticCharacter.CreateTrebuchet(pos));
    }
    public static void BuildStorage(Game game, Vector3 pos)
    {
        game.Resources.Limit *= 2;
    }
}
