using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// A field variable reference to a tile in a tileset.
/// </summary>
public readonly struct FieldTile() {
    /// <summary>
    /// The tileset's numeric id.
    /// </summary>
    public readonly int Tileset { get; init; } = 0;

    /// <summary>
    /// The tile's bounds in the tileset. <br/>
    /// Stored in pixel values, can be converted to tile information using <see cref="GetTileInfo"/>.
    /// </summary>
    public readonly Rect Rect { get; init; } = new(0, 0);

    internal FieldTile(JsonElementTilesetRectangle jsonElem) : this() {
        Tileset = jsonElem.TilesetID;
        Rect = new(jsonElem.X, jsonElem.Y, jsonElem.Width, jsonElem.Height);
    }

    /// <summary>
    /// Convert into tile data using a loaded world.
    /// </summary>
    /// <param name="world">The world instance.</param>
    /// <returns>The tile data, null if the tile is invalid.</returns>
    public LDTKTilesetTile? GetTileInfo(LDTKWorld world) {
        LDTKTileset? tileset = world.Tilesets.ByID(Tileset);
        if (tileset == null) { return null; }
        return GetTileInfo((LDTKTileset)tileset);
    }

    /// <summary>
    /// Convert into tile data using a tileset.
    /// </summary>
    /// <param name="world">The tileset instance.</param>
    /// <returns>The tile data, null if the tile is invalid.</returns>
    public LDTKTilesetTile? GetTileInfo(LDTKTileset tileset) => tileset.ByPosition(GridPosition(tileset));

    Point2 GridPosition(LDTKTileset tileset) => new(
        Calc.Floor(Rect.X / tileset.TileSize),
        Calc.Floor(Rect.X / tileset.TileSize)
    );
}