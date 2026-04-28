using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKTilesetTile {
    public readonly int ID;
    public readonly Point2 GridPosition;
    public readonly Subtexture Texture;
    public readonly string[] EnumValues;
    public readonly Dictionary<string, string> CustomDataEntries = [];

    internal LDTKTilesetTile(int id, Point2 position, Subtexture texture, string[] enums, string[] customs) {
        ID = id;
        GridPosition = position;
        Texture = texture;
        EnumValues = enums;
        foreach (string custom in customs) {
            if (custom == string.Empty) { continue; }
            string[] entries = custom.Split(":");
            string name = entries[0];
            string data = entries.Length == 1 ? string.Empty : custom[(name.Length + 1)..].Trim();
            CustomDataEntries.Add(name, data);
        }
    }
}