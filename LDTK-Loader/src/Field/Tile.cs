using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct FieldTile() {
    public readonly int Tileset { get; init; } = 0;
    public readonly Rect Rect { get; init; } = new(0, 0);

    internal FieldTile(JsonElementTilesetRectangle jsonElem) : this() {
        Tileset = jsonElem.TilesetID;
        Rect = new(jsonElem.X, jsonElem.Y, jsonElem.Width, jsonElem.Height);
    }
}