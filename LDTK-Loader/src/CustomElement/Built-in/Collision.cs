using System.Globalization;
using System.Numerics;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Configuration information for the collision loader.
/// </summary>
public struct LDTKCollisionConfig() {
    /// <summary>
    /// The id used to signify collision data in tile's custom data.
    /// </summary>
    public required string EntryName;

    /// <summary>
    /// The function for converting editor data into collider tags.
    /// </summary>
    public required Func<string, int?> TagFunction;

    /// <summary>
    /// The source of the editor data used for creating collider tags.
    /// </summary>
    public LDTKCollisionLoaderElement.TagSource CollisionTagType = LDTKCollisionLoaderElement.TagSource.TILESET_ENUM;

    /// <summary>
    /// The id used to signify collision tags in tile's custom data. <br/>
    /// Only used if <see cref="CollisionTagType"/> is set to <see cref="LDTKCollisionLoaderElement.CUSTOM_DATA"/>. <br/>
    /// Format: <br/>
    ///     <c>[tagEntryName]: [tag1] [tag2] [tag3] ...</c> <br/>
    /// Example: <br/>
    ///     <c>collisionTags: Ground Hazard Liquid</c>
    /// </summary>
    public string CustomDataCollisionTagName = "collisionTags";

    /// <summary>
    /// If the positions in the collision data are percents of the tile size or hard-coded coordinates.
    /// </summary>
    public bool PercentPositions = false;
}

/*
tags using a custom data entry or a tileset's enum flags. Supports multiple collision shapes:
- FULL: A rectangle collider the exact size of the tile. Syntax: `[ENTRY NAME]: FULL`
- RECT: A rectangle collider of any size. Syntax: `[ENTRY NAME]: RECT [x] [y] [width] [height]` ex: `collider: RECT 4 4 8 8`
- CIRCLE: A circular collider centered on its x/y position. Syntax: `[ENTRY NAME]: CIRCLE [x] [y] [radius]` ex `collider: CIRCLE 8 8 4`
- POLY: A convex polygon collider made of an abritrary bumber of points. Syntax: `[ENTRY NAME]: POLY [x1] [y1] [x2] [y2] ...` ex `collider: POLY 0 8 16 8 8 16 0 8`
*/

/// <summary>
/// Custom loader behavior for creating tile-based colliders.
/// Colliders are defined using a line in the tile's custom data. <br/>
/// Format: <br/>
///     <c>[entryName]: FULL</c> <br/>
///     <c>[entryName]: RECT [x] [y] [width] [height]</c> <br/>
///     <c>[entryName]: CIRCLE [x] [y] [radius]</c> <br/>
///     <c>[entryName]: POLY [x1] [y1] [x2] [y2] ...</c> <br/>
/// Example: <br/>
///     <c>collider: FULL</c> <br/>
///     <c>collider: RECT 4 4 8 8</c> <br/>
///     <c>collider: CIRCLE 8 8 4</c> <br/>
///     <c>collider: POLY 0 8 16 8 8 16 0 8</c> <br/>
/// </summary>
/// <param name="config">The collision config data.</param>
public class LDTKCollisionLoaderElement(LDTKCollisionConfig config) : LDTKCustomLoaderElement {
    public enum TagSource {
        TILESET_ENUM,
        CUSTOM_DATA
    }

    readonly LDTKCollisionConfig Config = config;

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