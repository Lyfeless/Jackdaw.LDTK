namespace Jackdaw.Loader.LDTK;

public readonly struct LDTKTilesetContainer {
    public readonly Dictionary<string, LDTKTileset> Entries = [];

    internal LDTKTilesetContainer(Assets assets, LDTKConfig config, JsonElementTilesetDefinition[] tilesets) {
        foreach (JsonElementTilesetDefinition data in tilesets) {
            LDTKTileset tileset = new(assets, data);
            if (config.CustomElements.CanProcessTilesets) { config.CustomElements.OnTilesetLoad(tileset); }
            Entries.Add(tileset.Name, tileset);
        }
    }

    public readonly LDTKTileset[] AllWithTag(string tag) => [.. Entries.Values.Where(e => e.Tags.Contains(tag))];
    public readonly LDTKTileset? ByName(string name) => Entries.GetValueOrDefault(name);
    public readonly LDTKTileset? ByID(int id) => Entries.Values.FirstOrDefault(e => e.ID == id);
}