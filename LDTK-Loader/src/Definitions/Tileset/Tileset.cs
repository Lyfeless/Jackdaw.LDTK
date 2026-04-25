using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKTileset {
    public readonly string Name;
    public readonly int ID;
    public readonly Subtexture Atlas;
    public readonly Point2 GridSize;
    public readonly int TileSize;
    public readonly string[] Tags;

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

    public int GetID(Point2 gridPosition) => GetID(gridPosition.X, gridPosition.Y);
    public int GetID(int x, int y) => (y * GridSize.X) + x;
    public Point2 GetGridPosition(int id) => new(id % GridSize.X, id / GridSize.Y);

    public LDTKTilesetTile? ByID(int id) => Tiles.GetValueOrDefault(id);
    public LDTKTilesetTile? ByPosition(Point2 gridPosition) => ByID(GetID(gridPosition));

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