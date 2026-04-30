namespace Jackdaw.Loader.LDTK;

/// <summary>
/// A container for storing and accessing a world's tileset information.
/// </summary>
public readonly struct LDTKTilesetContainer {
    /// <summary>
    /// All tilesets contained within the world.
    /// </summary>
    public readonly Dictionary<string, LDTKTileset> Entries = [];

    internal LDTKTilesetContainer(Assets assets, LDTKConfig config, JsonElementTilesetDefinition[] tilesets) {
        foreach (JsonElementTilesetDefinition data in tilesets) {
            LDTKTileset tileset = new(assets, data);
            if (config.CustomElements.CanProcessTilesets) { config.CustomElements.OnTilesetLoad(tileset); }
            Entries.Add(tileset.Name, tileset);
        }
    }

    /// <summary>
    /// Get all tilesets with the given editor sorting tag.
    /// </summary>
    /// <param name="tag">The tag to filter for.</param>
    /// <returns>All tilesets that have the tag.</returns>
    public readonly LDTKTileset[] AllWithTag(string tag) => [.. Entries.Values.Where(e => e.Tags.Contains(tag))];

    /// <summary>
    /// Find a tileset by its name id.
    /// </summary>
    /// <param name="name">The tileset's name.</param>
    /// <returns>The matching tileset, null if none exist.</returns>
    public readonly LDTKTileset? ByName(string name) => Entries.GetValueOrDefault(name);

    /// <summary>
    /// Find a tileset by its numeric id.
    /// </summary>
    /// <param name="name">The tileset's id.</param>
    /// <returns>The matching tileset, null if none exist.</returns>
    public readonly LDTKTileset? ByID(int id) => Entries.Values.FirstOrDefault(e => e.ID == id);
}