using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKTilesetTile {
    public readonly int ID;
    public readonly Point2 GridPosition;
    public readonly Subtexture Texture;
    public readonly string[] EnumValues;
    public readonly string[] CustomDataEntries;

    internal LDTKTilesetTile(int id, Point2 position, Subtexture texture, string[] enums, string[] customs) {
        ID = id;
        GridPosition = position;
        Texture = texture;
        EnumValues = enums;
        CustomDataEntries = customs;
    }
}