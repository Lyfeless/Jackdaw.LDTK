using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKTile {
    public readonly Point2 GridPosition;
    public readonly LDTKTilesetTile TilesetTile;
    public readonly TwoAxisFlip Flip;
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