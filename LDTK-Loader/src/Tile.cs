using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// A level tile instance.
/// </summary>
public readonly struct LDTKTile {
    /// <summary>
    /// The tile's position in its layer's grid.
    /// </summary>
    public readonly Point2 GridPosition;

    /// <summary>
    /// The tile's data from its tileset.
    /// </summary>
    public readonly LDTKTilesetTile TilesetTile;

    /// <summary>
    /// Information for how the tile should be flipped.
    /// </summary>
    public readonly TwoAxisFlip Flip;

    /// <summary>
    /// The editor opacity of the tile.
    /// </summary>
    public readonly float Opacity;

    internal LDTKTile(LDTKTileset tileset, JsonElementTileInstance data, JsonElementLayerInstance layer) {
        GridPosition = Position(data, layer);
        TilesetTile = (LDTKTilesetTile)tileset.ByID(data.TileID)!;
        Flip = new(data.Flip);
        Opacity = data.Alpha;
    }

    static Point2 Position(JsonElementTileInstance data, JsonElementLayerInstance layer) => new(
        data.LevelPosition[0] / layer.TileSize,
        data.LevelPosition[1] / layer.TileSize
    );
}