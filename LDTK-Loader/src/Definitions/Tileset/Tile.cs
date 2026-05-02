using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Information on a single tile in a tileset.
/// </summary>
public readonly struct LDTKTilesetTile {
    /// <summary>
    /// The tile's internal numeric id.
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// The tile's position on the tileset atlas in grid coordinates.
    /// </summary>
    public readonly Point2 GridPosition;

    /// <summary>
    /// The tile's individual texture.
    /// </summary>
    public readonly Subtexture Texture;

    /// <summary>
    /// All enum values applied to the tile. <br/>
    /// More info about the tileset's tagging enum is stored in the tileset object.
    /// </summary>
    public readonly string[] EnumValues;

    /// <summary>
    /// User-defined custom data about the tile. <br/>
    /// All lines of the tile's custom data are split into key value pairs using the first ':' in the line as a seperator. <br/>
    /// Any entry that doesn't have a seperator is stored as a key with an empty string value.
    /// </summary>
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