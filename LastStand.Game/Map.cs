using System.Collections.Frozen;
using System.Numerics;
using System.Text.Json.Serialization;
using static LastStand.Game.Extensions;

namespace LastStand.Game;

/// <summary>
/// References:
/// https://www.redblobgames.com/grids/hexagons/
/// https://www.redblobgames.com/pathfinding/a-star/introduction.html
/// </summary>
public class Map
{
    private static FrozenSet<Vector3> Neighbors = new HashSet<Vector3>() {
        new(+1, 0, -1), // bottom right
        new(+1, -1, 0), // top right
        new(0, -1, +1), // top
        new(-1, 0, +1), // top left
        new(-1, +1, 0), // bottom left
        new(0, +1, -1)  // bottom
    }.ToFrozenSet();
    private static FrozenDictionary<ushort, HashSet<Vector3>> AllNeighborsVersions = Enumerable.Range(0, 6).ToFrozenDictionary(x => (ushort)x, x => new HashSet<Vector3>([.. Neighbors.Skip(x).Take(Neighbors.Count - x), .. Neighbors.Take(x)]));
    public static FrozenDictionary<ushort, List<Vector3>> OrderedHexRings = Enumerable.Range(1, Constants.LargeMapSize).ToFrozenDictionary(x => (ushort)x, x => CreateMapRings((ushort)x).ToList());
    private static FrozenDictionary<ushort, FrozenSet<Vector3>> HexRings = Enumerable.Range(1, Constants.LargeMapSize).ToFrozenDictionary(x => (ushort)x, x => CreateMapRings((ushort)x).ToFrozenSet());
    private static FrozenDictionary<ushort, FrozenSet<Vector3>> HexSpirals = Enumerable.Range(1, Constants.LargeMapSize).ToFrozenDictionary(x => (ushort)x, x => CreateMapSpirals((ushort)x).ToFrozenSet());

    // Rings of specified distance
    private static HashSet<Vector3> CreateMapRings(ushort distance)
    {
        HashSet<Vector3> results = [];
        Vector3 tile = CubeScale(Neighbors.ElementAt(4), distance);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < distance; j++)
            {
                results.Add(tile);
                tile = Neighbors.ElementAt(i) + tile;
            }
        }
        return results;
    }
    private static Vector3 CubeScale(Vector3 startNeighbor, ushort distance)
    {
        return new Vector3(startNeighbor.X * distance, startNeighbor.Y * distance, startNeighbor.Z * distance);
    }
    // Filled in map of specified distance
    private static HashSet<Vector3> CreateMapSpirals(ushort distance)
    {
        HashSet<Vector3> results = [];
        for (int q = -distance; q <= +distance; q++)
        {
            for (int r = Math.Max(-distance, -q - distance); r <= Math.Min(+distance, -q + distance); r++)
            {
                results.Add(new Vector3(q, r, -q - r));
            }
        }
        return results;
    }
    public static IEnumerable<Vector3> YieldingMapSpirals(ushort distance)
    {
        yield return Vector3.Zero;
        var neighbors = AllNeighborsVersions[Random.Shared.NextUShort(0, 6)]; // Random direction so there is no bias for collecting resources
        for (ushort d = 1; d < distance; d++)
        {
            Vector3 tile = CubeScale(neighbors.ElementAt(4), d);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < d; j++)
                {
                    yield return tile;
                    tile = neighbors.ElementAt(i) + tile;
                }
            }
        }
    }

    private FrozenDictionary<Vector3, Tile> mapTiles;
    [JsonConverter(typeof(MapTilesJsonConverter))]
    public FrozenDictionary<Vector3, Tile> MapTiles
    {
        get { return mapTiles; }
        set
        {
            mapTiles = value;
            SetNeighbors();
        }
    }

    public ushort MapSize { get; set; } = Constants.StartMapSize;

    public void CreateMap()
    {
        MapSize = Constants.StartMapSize;
        var tiles = HexSpirals[MapSize];
        MapTiles = tiles.ToFrozenDictionary(x => x, x => new Tile(x, 1));
        MapTiles[Vector3.Zero].BuildBuilding(Building.Buildings[Constants.Base]);
    }
    public void IncreaseMapMedium()
    {
        if (MapSize >= Constants.MediumMapSize) throw new LastStandException("The map is already at medium size");
        var currentTiles = MapTiles.Values;
        var newTiles = GetMediumMapTiles().Select(x => new Tile(x, 2));
        HashSet<Tile> test = [.. currentTiles, .. newTiles];
        MapTiles = test.ToFrozenDictionary(x => x.Pos, x => x);
        MapSize = Constants.MediumMapSize;
    }
    public void IncreaseMapLarge()
    {
        if (MapSize >= Constants.LargeMapSize) throw new LastStandException("The map is already at medium size");
        var currentTiles = MapTiles.Values;
        var newTiles = GetLargeMapTiles().Select(x => new Tile(x, 3));
        HashSet<Tile> test = [.. currentTiles, .. newTiles];
        MapTiles = test.ToFrozenDictionary(x => x.Pos, x => x);
        MapSize = Constants.LargeMapSize;
    }
    private void SetNeighbors()
    {
        var tiles = MapTiles;
        foreach (var tile in tiles.Values)
        {
            HashSet<Tile> neighbors = [];
            foreach (var n in Neighbors)
            {
                if (tiles.TryGetValue(n + tile.Pos, out var t))
                {
                    neighbors.Add(t);
                }
            }
            tile.Neighbors = neighbors.ToFrozenSet();
        }
    }

    private static IEnumerable<Vector3> GetMediumMapTiles()
    {
        var tiles = new List<Vector3>();
        for (ushort i = Constants.StartMapSize + 1; i <= Constants.MediumMapSize; i++)
        {
            var ring = HexRings[i];
            tiles.AddRange(ring);
        }
        return tiles;
    }
    private static IEnumerable<Vector3> GetLargeMapTiles()
    {
        var tiles = new List<Vector3>();
        for (ushort i = Constants.MediumMapSize + 1; i <= Constants.LargeMapSize; i++)
        {
            var ring = HexRings[i];
            tiles.AddRange(ring);
        }
        return tiles;
    }

    public bool TryFindNearest(Vector3 start, Predicate<Tile> condition, out Vector3 target)
    {
        foreach (var p in YieldingMapSpirals(Constants.LargeMapSize * 2))
        {
            var pos = p + start;
            if (MapTiles.TryGetValue(pos, out var tile) && condition(tile))
            {
                target = pos;
                return true;
            }
        }
        target = Vector3.Zero;
        return false;
    }

    public List<Vector3> PathFind(Vector3 start, Vector3 target, uint distance)
    {
        PriorityQueue<Vector3, ushort> frontier = new();
        frontier.Enqueue(start, 0);
        Dictionary<Vector3, Vector3> came_from = [];
        Dictionary<Vector3, ushort> cost_so_far = [];
        came_from[start] = Vector3.Zero;
        cost_so_far[start] = 0;
        while (frontier.Count != 0)
        {
            var current = frontier.Dequeue();
            if (current == target) break;

            foreach (var tile in MapTiles[current].Neighbors)
            {
                ushort new_cost = (ushort)(cost_so_far[current] + 1);
                if (cost_so_far.ContainsKey(tile.Pos) is false || new_cost < cost_so_far[tile.Pos])
                {
                    cost_so_far[tile.Pos] = new_cost;
                    ushort priorty = (ushort)(new_cost + Heuristic(target, tile.Pos));
                    frontier.Enqueue(tile.Pos, priorty);
                    came_from[tile.Pos] = current;
                }
            }
        }

        List<Vector3> path = [target];
        while (path[^1] != start)
        {
            path.Add(came_from[path[^1]]);
        }
        path.Reverse();
        path.RemoveAt(0);
        return path[..(int)Math.Min(path.Count, distance)];
    }
    private static ushort Heuristic(Vector3 a, Vector3 b)
    {
        return (ushort)(Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z));
    }

    public bool BuildingExists(string building)
    {
        return MapTiles.Any(x => x.Value.Building == building);
    }
    public Tile GetBuildGround()
    {
        foreach (var target in YieldingMapSpirals(Constants.LargeMapSize))
        {
            var tile = MapTiles[target];
            if (tile.Building is null) return tile;
        }
        return MapTiles[Vector3.Zero];
    }

    public Vector3 GetRandomBorderPos()
    {
        var border = HexRings[MapSize].Items;
        return border[Random.Shared.Next(0, border.Length)];
    }
}

public class Tile(Vector3 pos, int tier)
{
    public int Tier { get; set; } = tier;

    [JsonConverter(typeof(VectorJsonConverter))]
    public Vector3 Pos { get; set; } = pos;

    [JsonIgnore]
    public FrozenSet<Tile> Neighbors { get; set; }

    public string? Building { get; set; }

    /// <summary>
    /// The resource on the tile
    /// If null then it has no resource and is considered grass
    /// Grass is populated to a randomly weighted resource
    /// </summary>
    public Resource? Resource { get; set; }

    public void Populate()
    {
        if (Building is not null) return;
        var val = Random.Shared.NextSingle();
        if (Resource is null)
        {
            if (Tier is 1)
            {
                Resource = val switch
                {
                    <= Constants.GrainGrowLimit1 => new SmallGrain() { Tile = this },
                    _ => new Tree() { Tile = this },
                };
            }
            else if (Tier is 2)
            {
                Resource = val switch
                {
                    <= Constants.GrainGrowLimit2 => new SmallGrain() { Tile = this },
                    <= Constants.WoodGrowLimit2 => new Tree() { Tile = this },
                    <= Constants.StoneGrowLimit2 => new StonePile() { Tile = this },
                    _ => new SteelMine() { Tile = this },
                };
            }
            else
            {
                Resource = val switch
                {
                    <= Constants.GrainGrowLimit3 => new SmallGrain() { Tile = this },
                    <= Constants.WoodGrowLimit3 => new Tree() { Tile = this },
                    <= Constants.StoneGrowLimit3 => new StonePile() { Tile = this },
                    _ => new SteelMine() { Tile = this },
                };
            }
        }
        else if (Resource is SmallGrain)
        {
            if (Neighbors.All(x => x.Resource?.ResourceType is ResourceType.Grain))
            {
                Resource = new MediumGrain(Resource.Amount) { Tile = this };
            }
        }
        else if (Resource is MediumGrain)
        {
            if (Neighbors.All(x => x.Resource is MediumGrain || x.Resource is LargeGrain))
            {
                Resource = new LargeGrain(Resource.Amount) { Tile = this };
            }
        }
        else if (Resource is Tree)
        {
            if (Neighbors.All(x => x.Resource?.ResourceType is ResourceType.Wood))
            {
                Resource = new Forest(Resource.Amount) { Tile = this };
            }
        }
    }

    public void BuildBuilding(Building building)
    {
        Building = building.Name;
        Resource = null;
    }
}