using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Layer instance data for a tile layer in a level.
/// </summary>
public readonly struct LDTKTileLayer {
    /// <summary>
    /// Generic layer information.
    /// </summary>
    public readonly LDTKLayer Metadata;

    /// <summary>
    /// The layer's grid dimensions.
    /// </summary>
    public readonly Point2 GridSize;

    /// <summary>
    /// The tileset the layer uses.
    /// </summary>
    public readonly LDTKTileset Tileset;

    /// <summary>
    /// The information of every tile on the layer.
    /// Multiple tiles are able to occupy the same grid position in some cases.
    /// </summary>
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