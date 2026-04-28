using System.Globalization;
using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public struct LDTCollisionConfig() {
    public required string EntryName;
    public required Func<string, int?> TagFunction;
    public LDTKCollisionLoaderElement.TagSource CollisionTagType = LDTKCollisionLoaderElement.TagSource.TILESET_ENUM;
    public string CustomDataCollisionTagName = "collisionTags";
    public bool PercentPositions = false;
}

public class LDTKCollisionLoaderElement(LDTCollisionConfig config) : LDTKCustomLoaderElement {
    public enum TagSource {
        TILESET_ENUM,
        CUSTOM_DATA
    }

    readonly LDTCollisionConfig Config = config;

    readonly Dictionary<(int, int), ITileCollider> colliders = [];

    public override bool CanProcessTilesets => true;
    public override bool CanModifyTileLayer => true;

    public override void OnTilesetLoad(LDTKTileset tileset) {
        int tilesetID = tileset.ID;
        int tileSize = tileset.TileSize;
        foreach ((int tileID, LDTKTilesetTile tile) in tileset.Tiles) {
            if (!tile.CustomDataEntries.TryGetValue(Config.EntryName, out string? data) || data == string.Empty) { continue; }
            ITileCollider? collider = ParseCollider(data, tileSize, GetTags(tile));
            if (collider == null) {
                Log.Warning($"LDTK Collider: Failed to parse collider {data}, skipping.");
                continue;
            }
            colliders.Add((tilesetID, tileID), collider);
        }
    }

    public override Actor OnTileLayerLoad(Actor actor, LDTKTileLayer layer) {
        int tilesetID = layer.Tileset.ID;
        int tileSize = layer.Tileset.TileSize;
        GridCollider grid = new(layer.GridSize, new(tileSize));

        bool addGrid = false;
        foreach (LDTKTile tile in layer.TileElements) {
            if (!colliders.TryGetValue((tilesetID, tile.TilesetTile.ID), out ITileCollider? collider)) { continue; }
            grid.Set(collider.Get(tileSize, tile.Flip), tile.GridPosition);
            addGrid = true;
        }

        if (addGrid) {
            actor.Components.Add(new CollisionComponent(actor.Game, grid));
        }

        return actor;
    }

    TagContainer GetTags(LDTKTilesetTile tile) => Config.CollisionTagType switch {
        TagSource.TILESET_ENUM => GetTags(tile.EnumValues),
        TagSource.CUSTOM_DATA => tile.CustomDataEntries.TryGetValue(Config.CustomDataCollisionTagName, out string? value)
            ? GetTags(value.Split(' '))
            : new(),
        _ => new()
    };

    TagContainer GetTags(string[] tags) {
        TagContainer container = new();
        foreach (string tag in tags) {
            int? value = Config.TagFunction(tag);
            if (value == null) { continue; }
            container.Add((int)value);
        }
        return container;
    }

    ITileCollider? ParseCollider(string data, int tileSize, TagContainer tags) {
        if (data == string.Empty) { return null; }
        string[] entries = data.Split(' ');

        float[] elements = new float[entries.Length - 1];
        for (int i = 0; i < elements.Length; ++i) {
            if (!float.TryParse(entries[i + 1], CultureInfo.InvariantCulture, out float value)) { return null; }
            if (Config.PercentPositions) { value *= tileSize; }
            elements[i] = value;
        }

        return entries[0].ToLower() switch {
            "full" => ParseRect([0, 0, tileSize, tileSize], tags),
            "rect" => ParseRect(elements, tags),
            "circle" => ParseCircle(elements, tags),
            "poly" => ParsePoly(elements, tags),
            _ => null
        };
    }

    static TileColliderRect? ParseRect(float[] data, TagContainer tags) {
        if (data.Length != 4) {
            Log.Warning($"LDTK Collider: Incorrect element count for rectangle collider {data}, skipping.");
            return null;
        }
        return new(data, tags);
    }

    static TileColliderCircle? ParseCircle(float[] data, TagContainer tags) {
        if (data.Length != 3) {
            Log.Warning($"LDTK Collider: Incorrect element count for circle collider {data}, skipping.");
            return null;
        }
        return new(data, tags);
    }

    static TileColliderPoly? ParsePoly(float[] data, TagContainer tags) {
        if (data.Length % 2 != 0) {
            Log.Warning($"LDTK Collider: Incorrect element count for polygon collider {data}, skipping.");
            return null;
        }
        return new(data, tags);
    }

    internal static Vector2 InvertPoint(Vector2 point, int tileSize, TwoAxisFlip flip) => new(
        flip.FlipX ? tileSize - point.X : point.X,
        flip.FlipY ? tileSize - point.Y : point.Y
    );
}

internal interface ITileCollider {
    public Collider Get(int tileSize, TwoAxisFlip flip);
}

internal readonly struct TileColliderRect(float[] data, TagContainer tags) : ITileCollider {
    readonly Rect rect = new(data[0], data[1], data[2], data[3]);

    public readonly Collider Get(int tileSize, TwoAxisFlip flip) {
        if (flip.Neither) { return new RectangleCollider(rect); }
        float x = flip.FlipX ? tileSize - rect.X - rect.Size.X : rect.X;
        float y = flip.FlipY ? tileSize - rect.Y - rect.Size.Y : rect.Y;
        return new RectangleCollider(new Rect(x, y, rect.Width, rect.Height)) { Tags = tags };
    }
}

internal readonly struct TileColliderCircle(float[] data, TagContainer tags) : ITileCollider {
    readonly Circle circle = new(data[0], data[1], data[2]);

    public readonly Collider Get(int tileSize, TwoAxisFlip flip) => new CircleCollider(flip.Neither
        ? circle
        : new Circle(LDTKCollisionLoaderElement.InvertPoint(circle.Position, tileSize, flip), circle.Radius)
    ) { Tags = tags };


}

internal readonly struct TileColliderPoly : ITileCollider {
    readonly Vector2[] points;
    readonly TagContainer tags;

    public TileColliderPoly(float[] data, TagContainer tags) {
        this.tags = tags;
        int halfLength = data.Length / 2;
        points = new Vector2[halfLength];
        for (int i = 0; i < halfLength; ++i) {
            int idx = i * 2;
            points[i] = new(data[idx], data[idx + 1]);
        }
    }

    public readonly Collider Get(int tileSize, TwoAxisFlip flip) => new PolygonCollider(new(flip.Neither
        ? points
        : [.. points.Select(e => LDTKCollisionLoaderElement.InvertPoint(e, tileSize, flip))]
    )) { Tags = tags };
}