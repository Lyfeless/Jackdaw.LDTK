using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Information for a tileset.
/// </summary>
public readonly struct LDTKTileset {
    /// <summary>
    /// The tileset's internal numeric id.
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// The tileset's name id.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The texture atlas for the tileset.
    /// </summary>
    public readonly Subtexture Atlas;

    /// <summary>
    /// The size of the tileset in grid coordinates.
    /// </summary>
    public readonly Point2 GridSize;

    /// <summary>
    /// The size of a single tile. <br/>
    /// All tiles are square, so this can be used for the width and height.
    /// </summary>
    public readonly int TileSize;

    /// <summary>
    /// Editor tags for sorting the tileset.
    /// </summary>
    public readonly string[] Tags;

    /// <summary>
    /// All tiles defined in the tileset.
    /// </summary>
    public readonly Dictionary<int, LDTKTilesetTile> Tiles = [];

    internal LDTKTileset(Assets assets, JsonElementTilesetDefinition data) {
        Name = data.Name;
        ID = data.ID;
        Atlas = GetAtlas(assets, data.TexturePath);
        GridSize = new(data.GridWidth, data.GridHeight);
        TileSize = data.TileSize;
        Tags = data.Tags;

        for (int x = 0; x < GridSize.X; ++x) {
            for (int y = 0; y < GridSize.Y; ++y) {
                Point2 gridPoint = new(x, y);
                int id = GetID(x, y);
                Tiles.Add(id, new(
                    id,
                    gridPoint,
                    Atlas.GetClipSubtexture(GetTextureRect(gridPoint, data.Padding, data.Spacing)),
                    [.. data.EnumTags.Where(e => e.IDs.Contains(id)).Select(e => e.Value)],
                    GetCustomData(id, data.CustomData)
                ));
            }
        }
    }

    /// <summary>
    /// Get a tile's numeric id from its grid position.
    /// </summary>
    /// <param name="gridPosition">The grid position of the tile.</param>
    /// <returns>The numeric id of the given position.</returns>
    public int GetID(Point2 gridPosition) => GetID(gridPosition.X, gridPosition.Y);

    /// <summary>
    /// Get a tile's numeric id from its grid position.
    /// </summary>
    /// <param name="x">The x coordinate of the tile.</param>
    /// <param name="y">The y coordinate of the tile.</param>
    /// <returns>The numeric id of the given position.</returns>
    public int GetID(int x, int y) => (y * GridSize.X) + x;

    /// <summary>
    /// Get a tile's grid position from its numeric id.
    /// </summary>
    /// <param name="id">The tile's numeric id.</param>
    /// <returns>The grid position of the givenm numeric id.</returns>
    public Point2 GetGridPosition(int id) => new(id % GridSize.X, id / GridSize.Y);

    /// <summary>
    /// Find a tile by its numeric id.
    /// </summary>
    /// <param name="id">The tile's numeric id.</param>
    /// <returns>The tile, null if it doesn't exist.</returns>
    public LDTKTilesetTile? ByID(int id) => Tiles.GetValueOrDefault(id);

    /// <summary>
    /// Find a tile by its grid position.
    /// </summary>
    /// <param name="id">The tile's grid position.</param>
    /// <returns>The tile, null if it doesn't exist.</returns>
    public LDTKTilesetTile? ByPosition(Point2 gridPosition) => ByID(GetID(gridPosition));

    /// <summary>
    /// Find a tile by its grid position.
    /// </summary>
    /// <param name="x">The x coordinate of the tile.</param>
    /// <param name="y">The y coordinate of the tile.</param>
    /// <returns>The tile, null if it doesn't exist.</returns>
    public LDTKTilesetTile? ByPosition(int x, int y) => ByID(GetID(x, y));

    static Subtexture GetAtlas(Assets assets, string path) {
        path = LDTKLoader.RemoveBacklinks(path);
        return assets.GetSubtexture(AssetProviderItem.FromString(path).Name);
    }

    Rect GetTextureRect(Point2 gridPoint, int padding, int spacing) => new(
        GetTextureAxis(gridPoint.X, padding, spacing),
        GetTextureAxis(gridPoint.Y, padding, spacing),
        TileSize,
        TileSize
    );

    int GetTextureAxis(int axis, int padding, int spacing) => padding + (axis * (TileSize + spacing));

    static string[] GetCustomData(int id, JsonElementTilesetCustomData[] data) {
        foreach (JsonElementTilesetCustomData element in data) {
            if (element.TileID == id) { return element.Data.Split("\n"); }
        }
        return [];
    }
}