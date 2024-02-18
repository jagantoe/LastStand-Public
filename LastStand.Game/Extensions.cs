using System.Collections.Frozen;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LastStand.Game;
public static class Extensions
{
    public static uint NextUInt(this Random random, int min, int max)
    {
        return (uint)random.Next(min, max);
    }
    public static ushort NextUShort(this Random random, int min, int max)
    {
        return (ushort)random.Next(min, max);
    }

    public static bool TryPeek<T>(this LinkedList<T> list, out T item)
    {
        item = default;
        if (list.Count is 0) return false;
        item = list.First.ValueRef;
        return true;
    }

    public static int Distance(this Vector3 vector, Vector3 target) => (int)((Math.Abs(vector.X - target.X) + Math.Abs(vector.Y - target.Y) + Math.Abs(vector.Z - target.Z)) / 2);
    public static Vector3 Rotate60Deg(this Vector3 vector) => new Vector3(-vector.Y, -vector.Z, -vector.X);
    public static string Serialize(this Vector3 vector) => $"{vector.X},{vector.Y},{vector.Z}";
    public static string MakeNameRandomized(this string name) => $"{name}_{Guid.NewGuid().ToString("n").Substring(0, 5)}";

    public class VectorJsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parts = reader.GetString().Split(",");
            return new Vector3(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Serialize());
        }
    }

    public class MapTilesJsonConverter : JsonConverter<FrozenDictionary<Vector3, Tile>>
    {
        public override FrozenDictionary<Vector3, Tile>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonDocument.TryParseValue(ref reader, out var doc))
            {
                return doc.Deserialize<List<Tile>>().TapTiles(x => { if (x.Resource is not null) x.Resource.Tile = x; }).ToFrozenDictionary(x => x.Pos, x => x);
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, FrozenDictionary<Vector3, Tile> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item.Value, options);
            }
            writer.WriteEndArray();
        }
    }
    public static List<Tile> TapTiles(this List<Tile> items, Action<Tile> action)
    {
        items.ForEach(action);
        return items;
    }
}
