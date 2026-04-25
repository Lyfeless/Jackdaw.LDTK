using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKTileLayer : ILayer {
    public LDTKLayer Metadata { get; init; }

    public readonly Point2 GridSize;
    public readonly LDTKTileset Tileset;
    public readonly LDTKTile[] TileElements;

    internal LDTKTileLayer(LDTKWorld world, JsonElementLayerInstance instance, JsonElementLayerDefinition definition, JsonElementTileInstance[] tiles) {
        Metadata = new(instance, definition);
        GridSize = new(instance.GridWidth, instance.GridHeight);
        // Hideous, but the only way this would crash is if the file has been tampered so it's safer than it looks
        Tileset = (LDTKTileset)world.Tilesets.ByID((int)instance.TilesetID!)!;
        LDTKTileset tileset = Tileset;
        TileElements = [.. tiles.Select(e => new LDTKTile(tileset, e, instance))];
    }
}